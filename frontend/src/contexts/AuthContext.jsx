import { createContext, useContext, useState } from 'react'
import axios from 'axios'

const AuthContext = createContext(null)
const API = import.meta.env.VITE_API_URL || 'http://localhost:8000'

// Axios auth interceptor
axios.interceptors.request.use((config) => {
  const token = localStorage.getItem('iim_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem('iim_user')
    return stored ? JSON.parse(stored) : null
  })

  const login = async (email, password) => {
    const res = await axios.post(`${API}/api/auth/login`, { email, password })
    const userData = { ...res.data, token: res.data.access_token }
    setUser(userData)
    localStorage.setItem('iim_user', JSON.stringify(userData))
    localStorage.setItem('iim_token', res.data.access_token)
    return userData
  }

  const logout = () => {
    setUser(null)
    localStorage.removeItem('iim_user')
    localStorage.removeItem('iim_token')
  }

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)
