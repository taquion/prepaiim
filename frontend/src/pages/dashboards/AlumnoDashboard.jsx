import { useState, useEffect } from 'react'
import axios from 'axios'
import DashboardLayout from '../../components/DashboardLayout'

const API = import.meta.env.VITE_API_URL || 'http://localhost:8000'

function GradeBar({ value }) {
  const pct = ((value / 10) * 100).toFixed(0)
  const color = value >= 8 ? 'bg-green-500' : value >= 7 ? 'bg-iim-gold' : 'bg-red-500'
  return (
    <div className="w-full bg-gray-100 rounded-full h-1.5 mt-1">
      <div className={`${color} h-1.5 rounded-full transition-all`} style={{ width: `${pct}%` }}></div>
    </div>
  )
}

export default function AlumnoDashboard() {
  const [perfil, setPerfil] = useState(null)
  const [calificaciones, setCalificaciones] = useState([])
  const [adeudos, setAdeudos] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    Promise.all([
      axios.get(`${API}/api/alumno/mi-perfil`).then((r) => setPerfil(r.data)),
      axios.get(`${API}/api/alumno/calificaciones`).then((r) => setCalificaciones(r.data)),
      axios.get(`${API}/api/alumno/adeudos`).then((r) => setAdeudos(r.data)),
    ]).finally(() => setLoading(false))
  }, [])

  const promedio =
    calificaciones.length
      ? (calificaciones.reduce((s, c) => s + c.calificacion, 0) / calificaciones.length).toFixed(1)
      : '—'

  const totalPendiente = adeudos.reduce((s, a) => s + (a.pendiente || 0), 0)

  return (
    <DashboardLayout title={perfil ? `Bienvenido, ${perfil.nombre}` : 'Mi Portal'}>
      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
        {[
          { label: 'Tetramestre', value: perfil?.tetramestre ? `${perfil.tetramestre}°` : '—', color: 'bg-iim-blue' },
          { label: 'Promedio', value: promedio, color: 'bg-iim-teal' },
          { label: 'Materias', value: calificaciones.length, color: 'bg-iim-light' },
          {
            label: 'Adeudo Pendiente',
            value: totalPendiente > 0 ? `$${totalPendiente.toLocaleString()}` : '✅',
            color: totalPendiente > 0 ? 'bg-red-500' : 'bg-green-500',
          },
        ].map((s) => (
          <div key={s.label} className={`${s.color} text-white rounded-xl p-4 shadow`}>
            <div className="text-2xl font-bold">{s.value}</div>
            <div className="text-sm opacity-90 mt-1">{s.label}</div>
          </div>
        ))}
      </div>

      {loading ? (
        <div className="text-center py-12 text-gray-400">Cargando información...</div>
      ) : (
        <div className="grid md:grid-cols-2 gap-6">
          {/* Calificaciones */}
          <div className="bg-white rounded-xl shadow p-6">
            <h2 className="text-lg font-bold text-iim-blue mb-2">Mis Calificaciones</h2>
            <div className="w-8 h-1 bg-iim-gold mb-4 rounded-full"></div>
            {calificaciones.length === 0 ? (
              <p className="text-gray-400 text-sm">Sin calificaciones registradas.</p>
            ) : (
              <ul className="space-y-3">
                {calificaciones.map((c, i) => (
                  <li key={i} className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="font-medium text-sm">{c.materia}</div>
                      <div className="text-xs text-gray-400">Tetramestre {c.tetramestre}</div>
                      <GradeBar value={c.calificacion} />
                    </div>
                    <div
                      className={`text-xl font-bold ml-4 ${
                        c.calificacion >= 8
                          ? 'text-green-600'
                          : c.calificacion >= 7
                          ? 'text-iim-gold'
                          : 'text-red-500'
                      }`}
                    >
                      {c.calificacion}
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </div>

          {/* Estado de cuenta */}
          <div className="bg-white rounded-xl shadow p-6">
            <h2 className="text-lg font-bold text-iim-blue mb-2">Estado de Cuenta</h2>
            <div className="w-8 h-1 bg-iim-gold mb-4 rounded-full"></div>
            {adeudos.length === 0 ? (
              <div className="text-center py-6">
                <div className="text-4xl mb-2">✅</div>
                <p className="text-green-600 font-medium">Sin adeudos pendientes</p>
              </div>
            ) : (
              <ul className="space-y-3">
                {adeudos.map((a, i) => (
                  <li key={i} className="p-3 bg-gray-50 rounded-lg">
                    <div className="flex justify-between items-start">
                      <div>
                        <div className="font-medium text-sm">{a.concepto}</div>
                        <div className="text-xs text-gray-400">
                          Total: ${a.monto?.toLocaleString()} · Pagado: ${a.pagado?.toLocaleString()}
                        </div>
                      </div>
                      <div
                        className={`text-sm font-bold ml-3 ${
                          a.pendiente > 0 ? 'text-red-600' : 'text-green-600'
                        }`}
                      >
                        {a.pendiente > 0 ? `$${a.pendiente.toLocaleString()}` : '✅'}
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      )}
    </DashboardLayout>
  )
}
