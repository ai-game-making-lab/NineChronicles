import os
from typing import NamedTuple, Optional, get_args

from dotenv import load_dotenv

from app.types import Env

load_dotenv(".env")


class Config(NamedTuple):
    github_token: Optional[str] = None
    key_passphrase: Optional[str] = None
    key_address: Optional[str] = None
    # Slack Bot API Token
    slack_token: Optional[str] = None
    runtime_url: str = "https://pipelines.actions.githubusercontent.com"
    runtime_token: Optional[str] = None
    # esigner path
    esigner_path: Optional[str] = None
    signing_secrets: Optional[dict] = None
    # env
    env: Env = "test"

    @classmethod
    def init(cls):
        _env = os.environ.get("ENV", "test")

        env_map = {v: v for v in get_args(Env)}
        try:
            env = env_map[_env]
        except KeyError:
            raise ValueError(f"Env should in {get_args(Env)}")

        signing_secrets = None
        try:
            signing_secrets = {
                "credential_id": os.environ["ESIGNER_CREDENTIAL_ID"],
                "username": os.environ["ESIGNER_USERNAME"],
                "password": os.environ["ESIGNER_PASSWORD"],
                "totp_secret": os.environ["ESIGNER_TOTP_SECRET"],
            }
        except KeyError:
            pass

        return cls(
            github_token=os.environ.get("GITHUB_TOKEN", ""),
            key_passphrase=os.environ.get("KEY_PASSPHRASE", ""),
            key_address=os.environ.get("KEY_ADDRESS", ""),
            slack_token=os.environ.get("SLACK_TOKEN", ""),
            runtime_url=os.environ.get(
                "ACTIONS_RUNTIME_URL",
                "https://pipelines.actions.githubusercontent.com",
            ),
            runtime_token=os.environ.get("ACTIONS_RUNTIME_TOKEN", ""),
            esigner_path=os.environ.get("ESIGNER_PATH", ""),
            signing_secrets=signing_secrets,
            env=env,
        )


config = Config.init()
