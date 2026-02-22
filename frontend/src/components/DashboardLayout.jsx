import { useAuth } from '../contexts/AuthContext'
import { useNavigate } from 'react-router-dom'
import { LogOut } from 'lucide-react'
import bear from '../assets/bear.jpg'

const roleLabels = {
  admin: '🏛️ Administrativo',
  maestro: '👨‍🏫 Maestro',
  alumno: '🎓 Alumno',
  padre: '👨‍👩‍👧 Padre de Familia',
}

export default function DashboardLayout({ children, title }) {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/')
  }

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Top bar */}
      <header className="bg-iim-blue text-white shadow-lg">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-3 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-full bg-iim-teal flex items-center justify-center overflow-hidden">
              <img
                src={bear}
                alt="Prepa IIM"
                className="w-full h-full object-cover"
                style={{ mixBlendMode: 'multiply' }}
              />
            </div>
            <div>
              <div className="font-semibold text-sm leading-tight">Instituto Intercultural Monterrey</div>
              <div className="text-xs text-iim-gold">{roleLabels[user?.role]}</div>
            </div>
          </div>
          <div className="flex items-center gap-4">
            <span className="text-sm hidden sm:block text-blue-100">{user?.nombre}</span>
            <button
              onClick={handleLogout}
              className="text-xs bg-white/10 hover:bg-white/20 px-3 py-1.5 rounded-lg transition flex items-center gap-1"
            >
              <LogOut size={12} /> <span className="hidden sm:inline">Salir</span>
            </button>
          </div>
        </div>
      </header>

      {/* Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {title && (
          <div className="mb-6">
            <h1 className="text-2xl font-bold text-iim-blue">{title}</h1>
            <div className="w-12 h-1 bg-iim-gold mt-2 rounded-full"></div>
          </div>
        )}
        {children}
      </main>
    </div>
  )
}
