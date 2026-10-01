"""Restore byte-identical original submission artifacts; Python standard library only."""
from pathlib import Path
import hashlib
import json
import os

ROOT = Path(__file__).resolve().parents[1]


def digest(path):
    result = hashlib.sha256()
    with path.open("rb") as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            result.update(block)
    return result.hexdigest()


def restore():
    manifest = json.loads((ROOT / "archive/large-artifacts/manifest.json").read_text())
    for artifact in manifest:
        destination = ROOT / artifact["output"]
        if destination.exists():
            if destination.stat().st_size == artifact["size"] and digest(destination) == artifact["sha256"]:
                print(f"Verified: {artifact['output']}")
                continue
            raise RuntimeError(f"Existing file differs; move it before restoring: {destination}")
        for part in artifact["parts"]:
            path = ROOT / part["path"]
            if path.stat().st_size != part["size"] or digest(path) != part["sha256"]:
                raise RuntimeError(f"Part is missing or damaged: {path}")
        destination.parent.mkdir(parents=True, exist_ok=True)
        temporary = destination.with_name(destination.name + ".restoring")
        try:
            with temporary.open("wb") as output:
                for part in artifact["parts"]:
                    with (ROOT / part["path"]).open("rb") as source:
                        for block in iter(lambda: source.read(1024 * 1024), b""):
                            output.write(block)
            if temporary.stat().st_size != artifact["size"] or digest(temporary) != artifact["sha256"]:
                raise RuntimeError(f"Restored artifact hash failed: {artifact['output']}")
            os.replace(temporary, destination)
        finally:
            temporary.unlink(missing_ok=True)
        print(f"Restored and verified: {artifact['output']}")


if __name__ == "__main__":
    restore()
