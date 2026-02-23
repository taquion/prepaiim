import { useState, useEffect } from 'react'
import axios from 'axios'
import DashboardLayout from '../../components/DashboardLayout'

const API = import.meta.env.VITE_API_URL || 'http://localhost:8000'

export default function AdminDashboard() {
  const [alumnos, setAlumnos] = useState([])
  const [adeudos, setAdeudos] = useState([])
  const [inscripciones, setInscripciones] = useState([])
  const [activeTab, setActiveTab] = useState('alumnos')
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    Promise.all([
      axios.get(`${API}/api/admin/alumnos`).then((r) => setAlumnos(r.data)),
      axios.get(`${API}/api/admin/adeudos`).then((r) => setAdeudos(r.data)),
      axios.get(`${API}/api/inscripciones`).then((r) => setInscripciones(r.data)).catch(() => {}),
    ]).finally(() => setLoading(false))
  }, [])

  const totalDeuda = adeudos.reduce((s, a) => s + (a.pendiente || 0), 0)
  const pendientes = adeudos.filter((a) => a.pendiente > 0).length

  return (
    <DashboardLayout title="Panel Administrativo">
      {/* Stats */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        {[
          { label: 'Total Alumnos', value: alumnos.length, color: 'bg-iim-blue' },
          { label: 'Con Adeudos', value: pendientes, color: pendientes > 0 ? 'bg-red-500' : 'bg-green-500' },
          { label: 'Deuda Total', value: `$${totalDeuda.toLocaleString()}`, color: 'bg-iim-gold' },
          { label: 'Al Corriente', value: adeudos.length - pendientes, color: 'bg-iim-teal' },
          { label: '📋 Leads Web', value: inscripciones.length, color: 'bg-orange-500' },
        ].map((s) => (
          <div key={s.label} className={`${s.color} text-white rounded-xl p-4 shadow`}>
            <div className="text-2xl font-bold">{s.value}</div>
            <div className="text-sm opacity-90 mt-1">{s.label}</div>
          </div>
        ))}
      </div>

      {/* Tabs */}
      <div className="bg-white rounded-xl shadow">
        <div className="flex border-b">
          {[
            { key: 'alumnos', label: '🎓 Alumnos' },
            { key: 'adeudos', label: '💰 Adeudos' },
            { key: 'inscripciones', label: '📋 Inscripciones Web' },
          ].map((tab) => (
            <button
              key={tab.key}
              onClick={() => setActiveTab(tab.key)}
              className={`px-6 py-3 font-medium text-sm transition ${
                activeTab === tab.key
                  ? 'border-b-2 border-iim-blue text-iim-blue'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>

        <div className="p-4 overflow-x-auto">
          {loading ? (
            <div className="text-center py-8 text-gray-400">Cargando...</div>
          ) : activeTab === 'alumnos' ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-gray-400 border-b text-xs uppercase tracking-wide">
                  <th className="pb-3 pr-4">Nombre</th>
                  <th className="pb-3 pr-4">Matrícula</th>
                  <th className="pb-3 pr-4 hidden md:table-cell">Email</th>
                  <th className="pb-3 pr-4">Tetramestre</th>
                  <th className="pb-3">Grupo</th>
                </tr>
              </thead>
              <tbody>
                {alumnos.length === 0 ? (
                  <tr><td colSpan={5} className="py-6 text-center text-gray-400">Sin alumnos registrados</td></tr>
                ) : alumnos.map((a) => (
                  <tr key={a.id} className="border-b hover:bg-gray-50 transition">
                    <td className="py-3 pr-4 font-medium">{a.nombre} {a.apellido}</td>
                    <td className="py-3 pr-4 text-gray-500 font-mono text-xs">{a.matricula}</td>
                    <td className="py-3 pr-4 text-gray-400 hidden md:table-cell">{a.email}</td>
                    <td className="py-3 pr-4">{a.tetramestre}°</td>
                    <td className="py-3">{a.grupo}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : activeTab === 'adeudos' ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-gray-400 border-b text-xs uppercase tracking-wide">
                  <th className="pb-3 pr-4">Alumno</th>
                  <th className="pb-3 pr-4">Concepto</th>
                  <th className="pb-3 pr-4">Total</th>
                  <th className="pb-3 pr-4">Pagado</th>
                  <th className="pb-3">Pendiente</th>
                </tr>
              </thead>
              <tbody>
                {adeudos.length === 0 ? (
                  <tr><td colSpan={5} className="py-6 text-center text-gray-400">Sin adeudos registrados</td></tr>
                ) : adeudos.map((a) => (
                  <tr key={a.id} className="border-b hover:bg-gray-50 transition">
                    <td className="py-3 pr-4 font-medium">{a.alumno}</td>
                    <td className="py-3 pr-4 text-gray-500">{a.concepto}</td>
                    <td className="py-3 pr-4">${a.monto?.toLocaleString()}</td>
                    <td className="py-3 pr-4 text-green-600">${a.pagado?.toLocaleString()}</td>
                    <td className={`py-3 font-bold ${a.pendiente > 0 ? 'text-red-600' : 'text-green-600'}`}>
                      {a.pendiente > 0 ? `$${a.pendiente.toLocaleString()}` : '✅ Pagado'}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : activeTab === 'inscripciones' ? (
            <div>
              <div className="flex items-center justify-between mb-4">
                <p className="text-sm text-gray-500">{inscripciones.length} lead(s) recibido(s) por el formulario web</p>
                <span className="text-xs bg-orange-100 text-orange-700 px-3 py-1 rounded-full font-medium">Leads de inscripción</span>
              </div>
              <table className="w-full text-sm">
                <thead>
                  <tr className="text-left text-gray-400 border-b text-xs uppercase tracking-wide">
                    <th className="pb-3 pr-4">Nombre</th>
                    <th className="pb-3 pr-4">Teléfono</th>
                    <th className="pb-3 pr-4 hidden md:table-cell">Secundaria</th>
                    <th className="pb-3">Fecha</th>
                  </tr>
                </thead>
                <tbody>
                  {inscripciones.length === 0 ? (
                    <tr><td colSpan={4} className="py-6 text-center text-gray-400">Sin solicitudes aún</td></tr>
                  ) : inscripciones.map((ins) => (
                    <tr key={ins.id} className="border-b hover:bg-gray-50 transition">
                      <td className="py-3 pr-4 font-medium">{ins.nombre}</td>
                      <td className="py-3 pr-4">
                        <a href={`https://wa.me/52${ins.telefono.replace(/\D/g,'')}`} target="_blank" rel="noopener noreferrer"
                           className="text-green-600 hover:underline font-medium flex items-center gap-1">
                          📱 {ins.telefono}
                        </a>
                      </td>
                      <td className="py-3 pr-4 text-gray-500 hidden md:table-cell">{ins.secundaria || '—'}</td>
                      <td className="py-3 text-gray-400 text-xs">{new Date(ins.created_at).toLocaleDateString('es-MX', { day:'2-digit', month:'short', year:'numeric', hour:'2-digit', minute:'2-digit' })}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : null}
        </div>
      </div>
    </DashboardLayout>
  )
}
