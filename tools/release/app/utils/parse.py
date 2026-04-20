import re
from typing import Iterable, Tuple

from app.exceptions import TagNotFoundError


def latest_tag(tags: Iterable[dict], rc: int, prefix: str = "") -> Tuple[str, str]:
    pattern = re.compile(rf"^{re.escape(prefix)}v{rc}-(\d+)$")
    latest = None

    for tag in tags:
        name = tag.get("name", "")
        match = pattern.match(name)
        if match is None:
            continue

        revision = int(match.group(1))
        sha = tag.get("commit", {}).get("sha")
        if sha is None:
            continue

        if latest is None or revision > latest[0]:
            latest = (revision, name, sha)

    if latest is None:
        raise TagNotFoundError(f"Tag not found: {prefix}v{rc}-*")

    _, name, sha = latest
    return name, sha
