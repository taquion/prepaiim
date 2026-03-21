import { useState, useEffect, useCallback } from 'react'
import axios from 'axios'
import DashboardLayout from '../../components/DashboardLayout'
import { Plus, Trash2, CheckCircle, BookOpen, Calendar, Link2, Loader, X } from 'lucide-react'

const API = import.meta.env.VITE_API_URL || ''

// ─── Sub-sección: Periodos ────────────────────────────────────────────────────
function PeriodosPanel() {
  const [periodos, setPeriodos] = useState([])
  const [nombre, setNombre] = useState('')
  const [semanas, setSemanas] = useState(14)
  const [saving, setSaving] = useState(false)

  const cargar = useCallback(() => {
    axios.get(`${API}/api/admin/periodos`).then(r => setPeriodos(r.data)).catch(() => {})
  }, [])

  useEffect(() => { cargar() }, [cargar])

  const crear = async () => {
    if (!nombre.trim()) return
    setSaving(true)
    await axios.post(`${API}/api/admin/periodos`, { nombre: nombre.trim(), semanas: Number(semanas) })
    setNombre('')
    setSemanas(14)
    cargar()
    setSaving(false)
  }

  const activar = async (id) => {
    await axios.put(`${API}/api/admin/periodos/${id}/activar`)
    cargar()
  }

  const eliminar = async (id) => {
    if (!confirm('¿Eliminar este periodo y todas sus asignaciones?')) return
    await axios.delete(`${API}/api/admin/periodos/${id}`)
    cargar()
  }

  return (
    <div>
      {/* Crear */}
      <div className="bg-gray-50 rounded-xl p-4 mb-5 flex flex-wrap gap-3 items-end">
        <div className="flex-1 min-w-[180px]">
          <label className="text-xs text-gray-500 font-semibold uppercase tracking-wide block mb-1">Nombre del periodo</label>
          <input
            value={nombre}
            onChange={e => setNombre(e.target.value)}
            placeholder="Ej. Ene-Abr 2026"
            className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-iim-blue"
          />
        </div>
        <div className="w-28">
          <label className="text-xs text-gray-500 font-semibold uppercase tracking-wide block mb-1">Semanas</label>
          <input
            type="number"
            value={semanas}
            onChange={e => setSemanas(e.target.value)}
            min={1} max={20}
            className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-iim-blue"
          />
        </div>
        <button
          onClick={crear}
          disabled={saving || !nombre.trim()}
          className="flex items-center gap-2 bg-iim-blue text-white px-4 py-2 rounded-lg text-sm font-semibold hover:bg-iim-blue/90 transition disabled:opacity-50"
        >
          {saving ? <Loader size={14} className="animate-spin" /> : <Plus size={14} />}
          Crear periodo
        </button>
      </div>

      {/* Lista */}
      {periodos.length === 0 ? (
        <p className="text-gray-400 text-sm text-center py-6">No hay periodos. Crea el primero.</p>
      ) : (
        <div className="space-y-2">
          {periodos.map(p => (
            <div
              key={p.id}
              className={`flex items-center justify-between p-3 rounded-xl border transition ${
                p.activo ? 'bg-green-50 border-green-200' : 'bg-white border-gray-100'
              }`}
            >
              <div className="flex items-center gap-3">
                {p.activo && <CheckCircle size={16} className="text-green-500 flex-shrink-0" />}
                <div>
                  <span className="font-semibold text-sm">{p.nombre}</span>
                  <span className="text-xs text-gray-400 ml-2">{p.semanas} semanas</span>
                  {p.activo && <span className="ml-2 text-xs font-bold text-green-600 bg-green-100 px-2 py-0.5 rounded-full">ACTIVO</span>}
                </div>
              </div>
              <div className="flex items-center gap-2">
                {!p.activo && (
                  <button
                    onClick={() => activar(p.id)}
                    className="text-xs text-iim-blue border border-iim-blue px-3 py-1 rounded-lg hover:bg-iim-blue hover:text-white transition"
                  >
                    Activar
                  </button>
                )}
                <button onClick={() => eliminar(p.id)} className="text-gray-300 hover:text-red-400 transition">
                  <Trash2 size={15} />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

// ─── Sub-sección: Materias ────────────────────────────────────────────────────
function MateriasPanel() {
  const [materias, setMaterias] = useState([])
  const [nombre, setNombre] = useState('')
  const [clave, setClave] = useState('')
  const [saving, setSaving] = useState(false)

  const cargar = useCallback(() => {
    axios.get(`${API}/api/admin/materias`).then(r => setMaterias(r.data)).catch(() => {})
  }, [])

  useEffect(() => { cargar() }, [cargar])

  const crear = async () => {
    if (!nombre.trim()) return
    setSaving(true)
    await axios.post(`${API}/api/admin/materias`, { nombre: nombre.trim(), clave: clave.trim() || null })
    setNombre('')
    setClave('')
    cargar()
    setSaving(false)
  }

  const eliminar = async (id) => {
    if (!confirm('¿Eliminar esta materia?')) return
    await axios.delete(`${API}/api/admin/materias/${id}`)
    cargar()
  }

  return (
    <div>
      {/* Crear */}
      <div className="bg-gray-50 rounded-xl p-4 mb-5 flex flex-wrap gap-3 items-end">
        <div className="flex-1 min-w-[180px]">
          <label className="text-xs text-gray-500 font-semibold uppercase tracking-wide block mb-1">Nombre de la materia</label>
          <input
            value={nombre}
            onChange={e => setNombre(e.target.value)}
            placeholder="Ej. Matemáticas"
            className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-iim-blue"
          />
        </div>
        <div className="w-36">
          <label className="text-xs text-gray-500 font-semibold uppercase tracking-wide block mb-1">Clave (opcional)</label>
          <input
            value={clave}
            onChange={e => setClave(e.target.value)}
            placeholder="MAT101"
            className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-iim-blue"
          />
        </div>
        <button
          onClick={crear}
          disabled={saving || !nombre.trim()}
          className="flex items-center gap-2 bg-iim-blue text-white px-4 py-2 rounded-lg text-sm font-semibold hover:bg-iim-blue/90 transition disabled:opacity-50"
        >
          {saving ? <Loader size={14} className="animate-spin" /> : <Plus size={14} />}
          Agregar materia
        </button>
      </div>

      {/* Lista */}
      {materias.length === 0 ? (
        <p className="text-gray-400 text-sm text-center py-6">No hay materias. Agrega la primera.</p>
      ) : (
        <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-2">
          {materias.map(m => (
            <div
              key={m.id}
              className="flex items-center justify-between bg-white border border-gray-100 rounded-xl p-3"
            >
              <div>
                <span className="font-semibold text-sm">{m.nombre}</span>
                {m.clave && <span className="text-xs text-gray-400 ml-2 font-mono">{m.clave}</span>}
              </div>
              <button onClick={() => eliminar(m.id)} className="text-gray-300 hover:text-red-400 transition ml-2">
                <Trash2 size={14} />
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

// ─── Sub-sección: Asignaciones ────────────────────────────────────────────────
function AsignacionesPanel() {
  const [asigs, setAsigs] = useState([])
  const [periodos, setPeriodos] = useState([])
  const [materias, setMaterias] = useState([])
  const [maestros, setMaestros] = useState([])
  const [filtPeriodo, setFiltPeriodo] = useState('')
  const [form, setForm] = useState({ maestro_id: '', materia_id: '', periodo_id: '', grupo: '' })
  const [saving, setSaving] = useState(false)

  const cargarTodo = useCallback(async () => {
    const [a, p, m, ms] = await Promise.all([
      axios.get(`${API}/api/admin/asignaciones`).then(r => r.data).catch(() => []),
      axios.get(`${API}/api/admin/periodos`).then(r => r.data).catch(() => []),
      axios.get(`${API}/api/admin/materias`).then(r => r.data).catch(() => []),
      axios.get(`${API}/api/admin/maestros-list`).then(r => r.data).catch(() => []),
    ])
    setAsigs(a)
    setPeriodos(p)
    setMaterias(m)
    setMaestros(ms)
    // Auto-select periodo activo
    const activo = p.find(x => x.activo)
    if (activo) setFiltPeriodo(String(activo.id))
  }, [])

  useEffect(() => { cargarTodo() }, [cargarTodo])

  const crear = async () => {
    if (!form.maestro_id || !form.materia_id || !form.periodo_id || !form.grupo.trim()) return
    setSaving(true)
    await axios.post(`${API}/api/admin/asignaciones`, {
      maestro_id: Number(form.maestro_id),
      materia_id: Number(form.materia_id),
      periodo_id: Number(form.periodo_id),
      grupo: form.grupo.trim(),
    })
    setForm({ maestro_id: '', materia_id: '', periodo_id: '', grupo: '' })
    cargarTodo()
    setSaving(false)
  }

  const eliminar = async (id) => {
    if (!confirm('¿Eliminar esta asignación?')) return
    await axios.delete(`${API}/api/admin/asignaciones/${id}`)
    cargarTodo()
  }

  const mostrar = filtPeriodo
    ? asigs.filter(a => String(a.periodo_id) === filtPeriodo)
    : asigs

  const formValido = form.maestro_id && form.materia_id && form.periodo_id && form.grupo.trim()

  return (
    <div>
      {/* Crear */}
      <div className="bg-gray-50 rounded-xl p-4 mb-5">
        <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-3">Nueva asignación</p>
        <div className="flex flex-wrap gap-3 items-end">
          <div className="flex-1 min-w-[160px]">
            <label className="text-xs text-gray-500 block mb-1">Maestro</label>
            <select
              value={form.maestro_id}
              onChange={e => setForm(f => ({ ...f, maestro_id: e.target.value }))}
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-iim-blue"
            >
              <option value="">Selecciona…</option>
              {maestros.map(m => <option key={m.id} value={m.id}>{m.nombre}</option>)}
            </select>
          </div>
          <div className="flex-1 min-w-[160px]">
            <label className="text-xs text-gray-500 block mb-1">Materia</label>
            <select
              value={form.materia_id}
              onChange={e => setForm(f => ({ ...f, materia_id: e.target.value }))}
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-iim-blue"
            >
              <option value="">Selecciona…</option>
              {materias.map(m => <option key={m.id} value={m.id}>{m.nombre}</option>)}
            </select>
          </div>
          <div className="flex-1 min-w-[140px]">
            <label className="text-xs text-gray-500 block mb-1">Periodo</label>
            <select
              value={form.periodo_id}
              onChange={e => setForm(f => ({ ...f, periodo_id: e.target.value }))}
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-iim-blue"
            >
              <option value="">Selecciona…</option>
              {periodos.map(p => (
                <option key={p.id} value={p.id}>{p.nombre}{p.activo ? ' ✓' : ''}</option>
              ))}
            </select>
          </div>
          <div className="w-28">
            <label className="text-xs text-gray-500 block mb-1">Grupo</label>
            <input
              value={form.grupo}
              onChange={e => setForm(f => ({ ...f, grupo: e.target.value }))}
              placeholder="1A"
              className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:border-iim-blue"
            />
          </div>
          <button
            onClick={crear}
            disabled={saving || !formValido}
            className="flex items-center gap-2 bg-iim-blue text-white px-4 py-2 rounded-lg text-sm font-semibold hover:bg-iim-blue/90 transition disabled:opacity-50"
          >
            {saving ? <Loader size={14} className="animate-spin" /> : <Plus size={14} />}
            Asignar
          </button>
        </div>
      </div>

      {/* Filtro */}
      <div className="flex items-center gap-3 mb-4 flex-wrap">
        <span className="text-xs text-gray-500 font-semibold">Filtrar por periodo:</span>
        <div className="flex gap-2 flex-wrap">
          <button
            onClick={() => setFiltPeriodo('')}
            className={`text-xs px-3 py-1 rounded-full border transition ${!filtPeriodo ? 'bg-iim-blue text-white border-iim-blue' : 'border-gray-200 text-gray-500 hover:border-iim-blue'}`}
          >
            Todos
          </button>
          {periodos.map(p => (
            <button
              key={p.id}
              onClick={() => setFiltPeriodo(String(p.id))}
              className={`text-xs px-3 py-1 rounded-full border transition ${String(filtPeriodo) === String(p.id) ? 'bg-iim-blue text-white border-iim-blue' : 'border-gray-200 text-gray-500 hover:border-iim-blue'}`}
            >
              {p.nombre}{p.activo ? ' ✓' : ''}
            </button>
          ))}
        </div>
      </div>

      {/* Lista */}
      {mostrar.length === 0 ? (
        <p className="text-gray-400 text-sm text-center py-6">Sin asignaciones en este periodo.</p>
      ) : (
        <div className="space-y-2">
          {mostrar.map(a => (
            <div key={a.id} className="flex items-center justify-between bg-white border border-gray-100 rounded-xl p-3">
              <div className="flex items-center gap-3 flex-wrap">
                <span className="font-semibold text-sm">{a.materia}</span>
                <span className="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded-full">Grupo {a.grupo}</span>
                <span className="text-xs text-gray-400">→</span>
                <span className="text-xs text-gray-600">{a.maestro}</span>
                {a.periodo && <span className="text-xs text-iim-blue bg-iim-blue/10 px-2 py-0.5 rounded-full">{a.periodo}</span>}
              </div>
              <button onClick={() => eliminar(a.id)} className="text-gray-300 hover:text-red-400 transition ml-2 flex-shrink-0">
                <Trash2 size={14} />
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

// ─── Panel Admin principal ────────────────────────────────────────────────────
export default function AdminDashboard() {
  const [alumnos, setAlumnos] = useState([])
  const [adeudos, setAdeudos] = useState([])
  const [inscripciones, setInscripciones] = useState([])
  const [activeTab, setActiveTab] = useState('alumnos')
  const [planSubTab, setPlanSubTab] = useState('periodos')
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
      <div className="grid grid-cols-2 lg:grid-cols-5 gap-4 mb-8">
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

      {/* Tabs principales */}
      <div className="bg-white rounded-xl shadow">
        <div className="flex border-b overflow-x-auto">
          {[
            { key: 'alumnos', label: '🎓 Alumnos' },
            { key: 'adeudos', label: '💰 Adeudos' },
            { key: 'inscripciones', label: '📋 Inscripciones Web' },
            { key: 'planeaciones', label: '📚 Planeaciones' },
          ].map((tab) => (
            <button
              key={tab.key}
              onClick={() => setActiveTab(tab.key)}
              className={`px-5 py-3 font-medium text-sm transition whitespace-nowrap flex-shrink-0 ${
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
          {loading && activeTab !== 'planeaciones' ? (
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
                      <td className="py-3 text-gray-400 text-xs">
                        {new Date(ins.created_at).toLocaleDateString('es-MX', { day:'2-digit', month:'short', year:'numeric', hour:'2-digit', minute:'2-digit' })}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : activeTab === 'planeaciones' ? (
            <div>
              {/* Sub-tabs */}
              <div className="flex gap-1 mb-5 bg-gray-100 p-1 rounded-xl w-fit">
                {[
                  { key: 'periodos', icon: <Calendar size={14} />, label: 'Periodos' },
                  { key: 'materias', icon: <BookOpen size={14} />, label: 'Materias' },
                  { key: 'asignaciones', icon: <Link2 size={14} />, label: 'Asignaciones' },
                ].map(t => (
                  <button
                    key={t.key}
                    onClick={() => setPlanSubTab(t.key)}
                    className={`flex items-center gap-1.5 px-4 py-1.5 rounded-lg text-sm font-medium transition ${
                      planSubTab === t.key
                        ? 'bg-white text-iim-blue shadow-sm'
                        : 'text-gray-500 hover:text-gray-700'
                    }`}
                  >
                    {t.icon} {t.label}
                  </button>
                ))}
              </div>

              {planSubTab === 'periodos' && <PeriodosPanel />}
              {planSubTab === 'materias' && <MateriasPanel />}
              {planSubTab === 'asignaciones' && <AsignacionesPanel />}
            </div>
          ) : null}
        </div>
      </div>
    </DashboardLayout>
  )
}
