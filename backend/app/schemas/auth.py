from pydantic import BaseModel, EmailStr
from app.models.user import RoleEnum


class LoginRequest(BaseModel):
    email: EmailStr
    password: str


class TokenResponse(BaseModel):
    access_token: str
    token_type: str = "bearer"
    role: RoleEnum
    nombre: str


class UserOut(BaseModel):
    id: int
    email: str
    nombre: str
    apellido: str
    role: RoleEnum
    activo: bool

    class Config:
        from_attributes = True
