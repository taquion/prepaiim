import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider, useAuth } from './contexts/AuthContext'
import Landing from './pages/Landing'
import Login from './pages/Login'
import AdminDashboard from './pages/dashboards/AdminDashboard'
import MaestroDashboard from './pages/dashboards/MaestroDashboard'
import AlumnoDashboard from './pages/dashboards/AlumnoDashboard'
import PadreDashboard from './pages/dashboards/PadreDashboard'

function ProtectedRoute({ children, role }) {
  const { user } = useAuth()
  if (!user) return <Navigate to="/login" replace />
  if (role && user.role !== role) return <Navigate to="/login" replace />
  return children
}

function RoleRedirect() {
  const { user } = useAuth()
  if (!user) return <Navigate to="/login" replace />
  const routes = { admin: '/admin', maestro: '/maestro', alumno: '/alumno', padre: '/padre' }
  return <Navigate to={routes[user.role] || '/login'} replace />
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Landing />} />
          <Route path="/login" element={<Login />} />
          <Route path="/portal" element={<RoleRedirect />} />
          <Route
            path="/admin"
            element={
              <ProtectedRoute role="admin">
                <AdminDashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/maestro"
            element={
              <ProtectedRoute role="maestro">
                <MaestroDashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/alumno"
            element={
              <ProtectedRoute role="alumno">
                <AlumnoDashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/padre"
            element={
              <ProtectedRoute role="padre">
                <PadreDashboard />
              </ProtectedRoute>
            }
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}
