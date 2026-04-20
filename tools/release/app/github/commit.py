from app.client import GithubClient


def get_latest_commit_hash_from_branch(client: GithubClient, branch: str) -> str:
    response = client.get_ref(f"heads/{branch}")
    return response["object"]["sha"]


def get_latest_commit_hash_from_tag(client: GithubClient, tag: str) -> str:
    for tags in client.get_tags():
        for item in tags:
            if item.get("name") == tag:
                return item["commit"]["sha"]

    raise ValueError(f"Tag not found: {tag}")
