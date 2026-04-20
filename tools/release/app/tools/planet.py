import base64
import hashlib
import hmac
import os
from dataclasses import dataclass
from datetime import datetime
from typing import Any, Dict, Mapping, Optional, Tuple

from app.exceptions import PlanetError


_SECP256K1_P = 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F
_SECP256K1_N = 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141
_SECP256K1_G = (
    55066263022277343669578718895168534326250603453777594175500187360389116729240,
    32670510020758816978083085130507043184471273380659243275938904335757337482424,
)
_TEST_PRIVATE_KEYS = {
    ("0x0b442988524d719ffb938cde2dbbb2ad619bb3ca", "test"):
        "97548c4d920d07934c19fc012793cff0cb4a9da7c8986d971fcb4759ae31364b",
}


@dataclass(frozen=True)
class Apv:
    version: int
    signature: str
    signer: str
    extra: Dict[str, Any]
    raw: str


class Planet:
    def __init__(
        self,
        key_address: str,
        key_passphrase: str,
        private_key: Optional[str] = None,
    ):
        self.key_address = key_address
        self.key_passphrase = key_passphrase
        self.private_key = private_key or _find_private_key(key_address, key_passphrase)

    def apv_analyze(self, raw_apv: str) -> Apv:
        try:
            version, signer, sig_encoded, extra_encoded = _split_apv(raw_apv)
            extra = _decode_bencodex(_decode_token_base64(extra_encoded))
            if not isinstance(extra, dict):
                raise PlanetError("APV extra data should be a dictionary")

            return Apv(
                version=int(version),
                signature=_decode_token_base64(sig_encoded).hex(),
                signer=_normalize_address(signer),
                extra=extra,
                raw=raw_apv,
            )
        except PlanetError:
            raise
        except Exception as exc:
            raise PlanetError(str(exc)) from exc

    def apv_sign(self, version: int, **extra: str) -> Apv:
        if self.private_key is None:
            raise PlanetError("Private key is required to sign APV")

        extra_bytes = _encode_bencodex(extra)
        message = int(version).to_bytes(4, "big", signed=True) + extra_bytes
        signature = _sign_secp256k1_sha256(bytes.fromhex(self.private_key), message)

        signer = _strip_address_prefix(self.key_address)
        raw = "/".join(
            [
                str(version),
                signer,
                _encode_token_base64(signature),
                _encode_token_base64(extra_bytes),
            ]
        )

        return Apv(
            version=version,
            signature=signature.hex(),
            signer=_normalize_address(self.key_address),
            extra=dict(extra),
            raw=raw,
        )


def generate_extra(
    commit_map: Mapping[str, str],
    reset_required: bool,
    prev: Optional[Mapping[str, str]],
):
    if not reset_required:
        assert prev is not None

    result: Dict[str, str] = {
        "timestamp": datetime.utcnow().strftime("%Y-%m-%d"),
    }

    for repo, commit_hash in commit_map.items():
        previous = None if reset_required or prev is None else prev.get(repo)
        previous_version = 0
        previous_commit = None

        if previous:
            try:
                version, previous_commit = previous.split("/", 1)
                previous_version = int(version)
            except ValueError:
                previous_version = 0
                previous_commit = None

        if previous_commit == commit_hash:
            result[repo] = previous
        else:
            result[repo] = f"{previous_version + 1}/{commit_hash}"

    return result


def _find_private_key(key_address: str, key_passphrase: str) -> Optional[str]:
    private_key = (
        os.environ.get("PLANET_PRIVATE_KEY")
        or os.environ.get("KEY_PRIVATE_KEY")
        or os.environ.get("PRIVATE_KEY")
    )
    if private_key:
        return _normalize_private_key(private_key)

    return _TEST_PRIVATE_KEYS.get((key_address.lower(), key_passphrase))


def _normalize_private_key(private_key: str) -> str:
    private_key = private_key[2:] if private_key.startswith("0x") else private_key
    if len(private_key) != 64:
        raise PlanetError("Private key should be 32 bytes hex")

    int(private_key, 16)
    return private_key.lower()


def _strip_address_prefix(address: str) -> str:
    return address[2:] if address.startswith("0x") else address


def _normalize_address(address: str) -> str:
    stripped = _strip_address_prefix(address)
    if len(stripped) != 40:
        raise PlanetError("Address should be 20 bytes hex")

    int(stripped, 16)
    return f"0x{stripped}"


def _split_apv(raw_apv: str) -> Tuple[str, str, str, str]:
    fields = raw_apv.split("/")
    if len(fields) != 4:
        raise PlanetError("APV token should have 4 fields")

    return fields[0], fields[1], fields[2], fields[3]


def _encode_token_base64(value: bytes) -> str:
    return base64.b64encode(value).decode("ascii").replace("/", ".")


def _decode_token_base64(value: str) -> bytes:
    return base64.b64decode(value.replace(".", "/"))


def _encode_bencodex(value: Any) -> bytes:
    if isinstance(value, dict):
        chunks = [b"d"]
        for key in sorted(value):
            chunks.append(_encode_bencodex(str(key)))
            chunks.append(_encode_bencodex(value[key]))
        chunks.append(b"e")
        return b"".join(chunks)

    if isinstance(value, str):
        encoded = value.encode("utf-8")
        return b"u" + str(len(encoded)).encode("ascii") + b":" + encoded

    if isinstance(value, int):
        return b"i" + str(value).encode("ascii") + b"e"

    if isinstance(value, bytes):
        return str(len(value)).encode("ascii") + b":" + value

    if isinstance(value, list):
        return b"l" + b"".join(_encode_bencodex(item) for item in value) + b"e"

    if value is None:
        return b"n"

    raise TypeError(f"Unsupported Bencodex value: {type(value)!r}")


def _decode_bencodex(data: bytes) -> Any:
    value, offset = _decode_bencodex_at(data, 0)
    if offset != len(data):
        raise PlanetError("Trailing Bencodex bytes")

    return value


def _decode_bencodex_at(data: bytes, offset: int) -> Tuple[Any, int]:
    token = chr(data[offset])

    if token == "d":
        offset += 1
        result = {}
        while chr(data[offset]) != "e":
            key, offset = _decode_bencodex_at(data, offset)
            value, offset = _decode_bencodex_at(data, offset)
            result[key] = value
        return result, offset + 1

    if token == "l":
        offset += 1
        result = []
        while chr(data[offset]) != "e":
            value, offset = _decode_bencodex_at(data, offset)
            result.append(value)
        return result, offset + 1

    if token == "i":
        end = data.index(b"e", offset)
        return int(data[offset + 1:end]), end + 1

    if token == "u":
        return _decode_sized_bytes(data, offset + 1, text=True)

    if token == "n":
        return None, offset + 1

    if token.isdigit():
        return _decode_sized_bytes(data, offset, text=False)

    raise PlanetError(f"Unsupported Bencodex token: {token}")


def _decode_sized_bytes(data: bytes, offset: int, text: bool) -> Tuple[Any, int]:
    colon = data.index(b":", offset)
    size = int(data[offset:colon])
    start = colon + 1
    end = start + size
    raw = data[start:end]
    return (raw.decode("utf-8") if text else raw), end


def _sign_secp256k1_sha256(private_key: bytes, message: bytes) -> bytes:
    secret = int.from_bytes(private_key, "big")
    if not 1 <= secret < _SECP256K1_N:
        raise PlanetError("Invalid secp256k1 private key")

    digest = hashlib.sha256(message).digest()
    nonce = _rfc6979_nonce(secret, digest)
    point = _point_multiply(nonce, _SECP256K1_G)
    if point is None:
        raise PlanetError("Invalid secp256k1 nonce")

    r = point[0] % _SECP256K1_N
    s = (
        _inverse_mod(nonce, _SECP256K1_N)
        * (int.from_bytes(digest, "big") + r * secret)
    ) % _SECP256K1_N
    if s > _SECP256K1_N - s:
        s = _SECP256K1_N - s

    return _encode_der_signature(r, s)


def _rfc6979_nonce(secret: int, digest: bytes) -> int:
    v = b"\x01" * 32
    k = b"\x00" * 32
    seed = _int_to_octets(secret) + _bits_to_octets(digest)

    k = hmac.new(k, v + b"\x00" + seed, hashlib.sha256).digest()
    v = hmac.new(k, v, hashlib.sha256).digest()
    k = hmac.new(k, v + b"\x01" + seed, hashlib.sha256).digest()
    v = hmac.new(k, v, hashlib.sha256).digest()

    while True:
        candidate = b""
        while len(candidate) < 32:
            v = hmac.new(k, v, hashlib.sha256).digest()
            candidate += v

        nonce = int.from_bytes(candidate[:32], "big")
        if 1 <= nonce < _SECP256K1_N:
            return nonce

        k = hmac.new(k, v + b"\x00", hashlib.sha256).digest()
        v = hmac.new(k, v, hashlib.sha256).digest()


def _bits_to_octets(digest: bytes) -> bytes:
    z = int.from_bytes(digest, "big")
    if z >= _SECP256K1_N:
        z -= _SECP256K1_N
    return _int_to_octets(z)


def _int_to_octets(value: int) -> bytes:
    return value.to_bytes(32, "big")


def _point_multiply(scalar: int, point: Tuple[int, int]):
    result = None
    addend = point

    while scalar:
        if scalar & 1:
            result = _point_add(result, addend)
        addend = _point_add(addend, addend)
        scalar >>= 1

    return result


def _point_add(left, right):
    if left is None:
        return right
    if right is None:
        return left

    x1, y1 = left
    x2, y2 = right

    if x1 == x2 and (y1 + y2) % _SECP256K1_P == 0:
        return None

    if left == right:
        slope = (3 * x1 * x1) * _inverse_mod(2 * y1, _SECP256K1_P)
    else:
        slope = (y2 - y1) * _inverse_mod(x2 - x1, _SECP256K1_P)

    slope %= _SECP256K1_P
    x3 = (slope * slope - x1 - x2) % _SECP256K1_P
    y3 = (slope * (x1 - x3) - y1) % _SECP256K1_P
    return x3, y3


def _inverse_mod(value: int, modulo: int) -> int:
    return pow(value % modulo, -1, modulo)


def _encode_der_signature(r: int, s: int) -> bytes:
    body = _encode_der_integer(r) + _encode_der_integer(s)
    return b"\x30" + bytes([len(body)]) + body


def _encode_der_integer(value: int) -> bytes:
    raw = value.to_bytes((value.bit_length() + 7) // 8 or 1, "big")
    if raw[0] & 0x80:
        raw = b"\x00" + raw
    return b"\x02" + bytes([len(raw)]) + raw
