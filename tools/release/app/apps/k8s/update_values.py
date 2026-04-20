from typing import NamedTuple

import yaml


class ImageMetadata(NamedTuple):
    repo: str
    source_type: str
    source_value: str


def extract_image_metadata(value: str) -> ImageMetadata:
    repo, source = value.split("/from ", 1)
    source_type, source_value = source.split(" ", 1)
    return ImageMetadata(repo, source_type, source_value)


def update_image_tag(contents: str, *, repo_to_change: str, tag_to_change: str) -> str:
    data = yaml.safe_load(contents)
    _update_image_tag(data, repo_to_change, tag_to_change)
    return yaml.safe_dump(data, sort_keys=False)


def _update_image_tag(value, repo_to_change: str, tag_to_change: str) -> None:
    if isinstance(value, dict):
        if _matches_repository(value.get("repository"), repo_to_change) and "tag" in value:
            value["tag"] = tag_to_change

        for child in value.values():
            _update_image_tag(child, repo_to_change, tag_to_change)
    elif isinstance(value, list):
        for child in value:
            _update_image_tag(child, repo_to_change, tag_to_change)


def _matches_repository(repository: str, repo_to_change: str) -> bool:
    if not isinstance(repository, str):
        return False

    return repository == repo_to_change or repository.rsplit("/", 1)[-1] == repo_to_change
