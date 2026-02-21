from sqlalchemy import Column, Integer, String, Boolean, DateTime, Enum
from sqlalchemy.orm import relationship
from datetime import datetime
import enum
from app.core.database import Base


class RoleEnum(str, enum.Enum):
    admin = "admin"
    maestro = "maestro"
    alumno = "alumno"
    padre = "padre"


class User(Base):
    __tablename__ = "users"
    id = Column(Integer, primary_key=True, index=True)
    email = Column(String, unique=True, index=True, nullable=False)
    nombre = Column(String, nullable=False)
    apellido = Column(String, nullable=False)
    hashed_password = Column(String, nullable=False)
    role = Column(Enum(RoleEnum), nullable=False)
    activo = Column(Boolean, default=True)
    created_at = Column(DateTime, default=datetime.utcnow)

    alumno_profile = relationship("Alumno", back_populates="user", uselist=False)
    maestro_profile = relationship("Maestro", back_populates="user", uselist=False)
    padre_profile = relationship("Padre", back_populates="user", uselist=False)
