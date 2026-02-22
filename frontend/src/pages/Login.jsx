import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { ArrowLeft } from 'lucide-react'
import bearWhite from '../assets/bear_white.png'

export default function Login() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError('')
    try {
      const user = await login(email, password)
      const routes = { admin: '/admin', maestro: '/maestro', alumno: '/alumno', padre: '/padre' }
      navigate(routes[user.role] || '/')
    } catch {
      setError('Credenciales incorrectas. Verifica tu email y contraseña.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen iim-gradient flex flex-col items-center justify-center px-4">
      {/* Logo */}
      <Link to="/" className="mb-8 flex flex-col items-center gap-3 group">
        <div className="w-20 h-20 flex items-center justify-center group-hover:opacity-90 transition">
          <img src={bearWhite} alt="PrepaIIM" className="w-full h-full object-contain" />
        </div>
        <div className="text-white text-center">
          <div className="text-xl font-bold">Instituto Intercultural</div>
          <div className="text-iim-gold text-sm font-medium">Monterrey, Pte.</div>
        </div>
      </Link>

      {/* Card */}
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md p-8">
        <div className="mb-6">
          <h2 className="text-2xl font-bold text-iim-blue text-center">Acceso al Portal</h2>
          <div className="w-12 h-1 bg-iim-gold mx-auto mt-2"></div>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg mb-4 text-sm">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Correo electrónico
            </label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="w-full border border-gray-300 rounded-lg px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-iim-blue focus:border-transparent transition"
              placeholder="usuario@iim.edu.mx"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Contraseña
            </label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="w-full border border-gray-300 rounded-lg px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-iim-blue focus:border-transparent transition"
              placeholder="••••••••"
            />
          </div>
          <button
            type="submit"
            disabled={loading}
            className="w-full bg-iim-blue hover:bg-iim-dark text-white font-bold py-3 rounded-lg transition disabled:opacity-50 mt-2"
          >
            {loading ? 'Entrando...' : 'Ingresar'}
          </button>
        </form>

        <div className="mt-6 pt-4 border-t border-gray-100 text-center">
          <p className="text-xs text-gray-400">Portal oficial del Instituto Intercultural Monterrey</p>
        </div>
      </div>

      <Link to="/" className="mt-6 text-blue-200 hover:text-white text-sm flex items-center gap-1 transition">
        <ArrowLeft size={14} /> Volver al sitio
      </Link>
    </div>
  )
}
