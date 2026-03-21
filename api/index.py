"""
Vercel serverless entry point — wraps the FastAPI app from backend/.
"""
import sys
from pathlib import Path

# Add backend/ to Python path so "from app.…" imports work
sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "backend"))

from main import app  # noqa — re-export for Vercel
