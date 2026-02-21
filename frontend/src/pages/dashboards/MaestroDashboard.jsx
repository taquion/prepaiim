import { useState, useEffect } from 'react'
import axios from 'axios'
import DashboardLayout from '../../components/DashboardLayout'
import { BookOpen, PlusCircle, BarChart2, Upload } from 'lucide-react'

const API = import.meta.env.VITE_API_URL || 'http://localhost:8000'

export default function MaestroDashboard() {
  const [materias, setMaterias] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    axios
      .get(`${API}/api/maestro/mis-materias`)
      .then((r) => setMaterias(r.data))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  return (
    <DashboardLayout title="Panel del Maestro">
      <div className="grid md:grid-cols-2 gap-6">
        {/* Materias */}
        <div className="bg-white rounded-xl shadow p-6">
          <div className="flex items-center gap-2 mb-4">
            <BookOpen size={18} className="text-iim-blue" />
            <h2 className="text-lg font-bold text-iim-blue">Mis Materias</h2>
          </div>
          <div className="w-8 h-1 bg-iim-gold mb-4 rounded-full"></div>

          {loading ? (
            <p className="text-gray-400 text-sm">Cargando...</p>
          ) : materias.length === 0 ? (
            <p className="text-gray-400 text-sm">No hay materias asignadas aún.</p>
          ) : (
            <ul className="space-y-2">
              {materias.map((m) => (
                <li
                  key={m.id}
                  className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg hover:bg-blue-50 transition"
                >
                  <div className="w-2 h-8 rounded-full bg-iim-blue flex-shrink-0"></div>
                  <div>
                    <div className="font-medium text-sm">{m.nombre}</div>
                    <div className="text-xs text-gray-400">
                      Tetramestre {m.tetramestre} — Grupo {m.grupo}
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>

        {/* Acciones rápidas */}
        <div className="bg-white rounded-xl shadow p-6">
          <h2 className="text-lg font-bold text-iim-blue mb-4">Acciones Rápidas</h2>
          <div className="w-8 h-1 bg-iim-gold mb-4 rounded-full"></div>
          <div className="space-y-3">
            {[
              {
                icon: <PlusCircle size={16} />,
                label: 'Nueva tarea',
                desc: 'Asignar tarea a un grupo',
                color: 'hover:border-iim-blue hover:bg-iim-blue',
              },
              {
                icon: <BarChart2 size={16} />,
                label: 'Vaciar calificaciones',
                desc: 'Capturar notas del periodo',
                color: 'hover:border-iim-teal hover:bg-iim-teal',
              },
              {
                icon: <Upload size={16} />,
                label: 'Subir contenido',
                desc: 'Material de clase o exámenes',
                color: 'hover:border-iim-gold hover:bg-iim-gold',
              },
            ].map((a) => (
              <button
                key={a.label}
                className={`w-full text-left p-3 border border-gray-200 rounded-lg ${a.color} hover:text-white hover:border-transparent transition group`}
              >
                <div className="flex items-center gap-2 font-medium text-sm">
                  {a.icon}
                  {a.label}
                </div>
                <div className="text-xs text-gray-400 group-hover:text-white/80 mt-0.5 pl-6">
                  {a.desc}
                </div>
              </button>
            ))}
          </div>
        </div>
      </div>
    </DashboardLayout>
  )
}
