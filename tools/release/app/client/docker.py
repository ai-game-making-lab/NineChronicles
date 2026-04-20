from app.client.session import BaseUrlSession
from app.exceptions import DockerImageNotFoundError


DOCKER_HUB_BASE_URL = "https://hub.docker.com"


class DockerClient:
    def __init__(self, namespace: str) -> None:
        self.namespace = namespace
        self._session = BaseUrlSession(DOCKER_HUB_BASE_URL)

    def check_image_exists(self, repo: str, tag: str) -> dict:
        response = self._session.get(
            f"/v2/namespaces/{self.namespace}/repositories/{repo}/tags/{tag}"
        )

        if response.status_code == 404:
            raise DockerImageNotFoundError(f"{self.namespace}/{repo}:{tag}")

        response.raise_for_status()
        return response.json()
