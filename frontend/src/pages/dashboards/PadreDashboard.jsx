import { useState, useEffect } from 'react'
import axios from 'axios'
import DashboardLayout from '../../components/DashboardLayout'

const API = import.meta.env.VITE_API_URL || 'http://localhost:8000'

export default function PadreDashboard() {
  const [hijos, setHijos] = useState([])
  const [selectedHijo, setSelectedHijo] = useState(null)
  const [calificaciones, setCalificaciones] = useState([])
  const [adeudos, setAdeudos] = useState([])
  const [loading, setLoading] = useState(true)
  const [detailLoading, setDetailLoading] = useState(false)

  useEffect(() => {
    axios
      .get(`${API}/api/padre/mis-hijos`)
      .then((r) => {
        setHijos(r.data)
        if (r.data.length > 0) loadHijoData(r.data[0])
      })
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  const loadHijoData = (hijo) => {
    setSelectedHijo(hijo)
    setDetailLoading(true)
    Promise.all([
      axios.get(`${API}/api/padre/hijo/${hijo.id}/calificaciones`).then((r) => setCalificaciones(r.data)),
      axios.get(`${API}/api/padre/hijo/${hijo.id}/adeudos`).then((r) => setAdeudos(r.data)),
    ]).finally(() => setDetailLoading(false))
  }

  const promedio =
    calificaciones.length
      ? (calificaciones.reduce((s, c) => s + c.calificacion, 0) / calificaciones.length).toFixed(1)
      : '—'

  return (
    <DashboardLayout title="Portal para Padres">
      {loading ? (
        <div className="text-center py-12 text-gray-400">Cargando...</div>
      ) : hijos.length === 0 ? (
        <div className="bg-white rounded-xl shadow p-8 text-center text-gray-400">
          No hay alumnos vinculados a tu cuenta. Contacta a la administración.
        </div>
      ) : (
        <>
          {/* Selector hijo */}
          {hijos.length > 1 && (
            <div className="flex gap-2 mb-6 flex-wrap">
              {hijos.map((h) => (
                <button
                  key={h.id}
                  onClick={() => loadHijoData(h)}
                  className={`px-4 py-2 rounded-lg font-medium text-sm transition ${
                    selectedHijo?.id === h.id
                      ? 'bg-iim-blue text-white shadow'
                      : 'bg-white text-iim-blue border border-iim-blue hover:bg-blue-50'
                  }`}
                >
                  {h.nombre} {h.apellido}
                </button>
              ))}
            </div>
          )}

          {selectedHijo && (
            <>
              {/* Info card */}
              <div className="bg-iim-blue text-white rounded-xl p-5 mb-6 flex flex-wrap gap-4 items-center justify-between">
                <div>
                  <div className="text-xl font-bold">
                    {selectedHijo.nombre} {selectedHijo.apellido}
                  </div>
                  <div className="text-blue-200 text-sm">
                    Matrícula: {selectedHijo.matricula} · Tetramestre {selectedHijo.tetramestre}°
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-3xl font-extrabold text-iim-gold">{promedio}</div>
                  <div className="text-xs text-blue-200">Promedio general</div>
                </div>
              </div>

              {detailLoading ? (
                <div className="text-center py-8 text-gray-400">Cargando detalle...</div>
              ) : (
                <div className="grid md:grid-cols-2 gap-6">
                  {/* Calificaciones */}
                  <div className="bg-white rounded-xl shadow p-6">
                    <h2 className="text-lg font-bold text-iim-blue mb-2">Calificaciones</h2>
                    <div className="w-8 h-1 bg-iim-gold mb-4 rounded-full"></div>
                    {calificaciones.length === 0 ? (
                      <p className="text-gray-400 text-sm">Sin calificaciones registradas.</p>
                    ) : (
                      <ul className="divide-y divide-gray-100">
                        {calificaciones.map((c, i) => (
                          <li key={i} className="flex justify-between items-center py-2">
                            <div>
                              <span className="text-sm font-medium">{c.materia}</span>
                              <span className="text-xs text-gray-400 ml-2">T{c.tetramestre}</span>
                            </div>
                            <span
                              className={`font-bold text-sm ${
                                c.calificacion >= 8
                                  ? 'text-green-600'
                                  : c.calificacion >= 7
                                  ? 'text-iim-gold'
                                  : 'text-red-500'
                              }`}
                            >
                              {c.calificacion}
                            </span>
                          </li>
                        ))}
                      </ul>
                    )}
                  </div>

                  {/* Adeudos */}
                  <div className="bg-white rounded-xl shadow p-6">
                    <h2 className="text-lg font-bold text-iim-blue mb-2">Estado de Cuenta</h2>
                    <div className="w-8 h-1 bg-iim-gold mb-4 rounded-full"></div>
                    {adeudos.length === 0 ? (
                      <div className="text-center py-4">
                        <div className="text-3xl mb-2">✅</div>
                        <p className="text-green-600 font-medium text-sm">Sin adeudos pendientes</p>
                      </div>
                    ) : (
                      <ul className="space-y-2">
                        {adeudos.map((a, i) => (
                          <li key={i} className="p-3 bg-gray-50 rounded-lg">
                            <div className="flex justify-between items-start">
                              <div>
                                <div className="font-medium text-sm">{a.concepto}</div>
                                <div className="text-xs text-gray-400">
                                  Total: ${a.monto?.toLocaleString()}
                                </div>
                              </div>
                              <div
                                className={`text-sm font-bold ${
                                  a.pendiente > 0 ? 'text-red-600' : 'text-green-600'
                                }`}
                              >
                                {a.pendiente > 0
                                  ? `$${a.pendiente.toLocaleString()} pendiente`
                                  : '✅ Pagado'}
                              </div>
                            </div>
                          </li>
                        ))}
                      </ul>
                    )}
                  </div>
                </div>
              )}
            </>
          )}
        </>
      )}
    </DashboardLayout>
  )
}
