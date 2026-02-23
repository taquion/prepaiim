import { useState, useEffect, useCallback } from 'react'
import axios from 'axios'
import DashboardLayout from '../../components/DashboardLayout'
import {
  BookOpen, ChevronRight, ChevronLeft, CheckCircle,
  Clock, Save, Loader, AlertCircle, FileText
} from 'lucide-react'

const API = import.meta.env.VITE_API_URL || 'http://localhost:8000'

// ─── Vista 1: Tarjetas de materias ────────────────────────────────────────────
function MisMaterias({ onSelectMateria }) {
  const [materias, setMaterias] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    axios.get(`${API}/api/maestro/mis-materias`)
      .then(r => setMaterias(r.data))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  const colores = [
    'from-blue-600 to-blue-800',
    'from-teal-600 to-teal-800',
    'from-indigo-600 to-indigo-800',
    'from-sky-600 to-sky-800',
    'from-violet-600 to-violet-800',
    'from-cyan-600 to-cyan-800',
  ]

  if (loading) return (
    <div className="flex items-center justify-center h-48 text-gray-400">
      <Loader size={24} className="animate-spin mr-2" /> Cargando materias…
    </div>
  )

  if (materias.length === 0) return (
    <div className="flex flex-col items-center justify-center h-48 text-center">
      <BookOpen size={40} className="text-gray-300 mb-3" />
      <p className="text-gray-500 font-medium">No hay materias asignadas aún</p>
      <p className="text-gray-400 text-sm mt-1">El administrador debe asignarte materias en el periodo activo</p>
    </div>
  )

  return (
    <div>
      {materias[0]?.periodo && (
        <div className="mb-6 inline-flex items-center gap-2 bg-iim-blue/10 text-iim-blue text-sm font-medium px-4 py-2 rounded-full">
          <Clock size={14} />
          Periodo activo: <span className="font-bold">{materias[0].periodo}</span>
        </div>
      )}
      <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-5">
        {materias.map((m, i) => {
          const pct = Math.round((m.planeaciones_completadas / m.semanas) * 100)
          return (
            <button
              key={m.asignacion_id}
              onClick={() => onSelectMateria(m)}
              className="text-left bg-white rounded-2xl shadow-sm border border-gray-100 hover:shadow-md hover:border-iim-blue/30 transition-all group overflow-hidden"
            >
              {/* Color header */}
              <div className={`bg-gradient-to-r ${colores[i % colores.length]} p-5 text-white`}>
                <div className="text-xs font-medium opacity-75 mb-1 uppercase tracking-wider">
                  {m.clave || 'Materia'} · Grupo {m.grupo}
                </div>
                <div className="font-bold text-lg leading-tight">{m.nombre}</div>
              </div>
              {/* Body */}
              <div className="p-4">
                <div className="flex items-center justify-between mb-2">
                  <span className="text-xs text-gray-500">
                    {m.planeaciones_completadas}/{m.semanas} semanas listas
                  </span>
                  <span className="text-xs font-bold text-iim-blue">{pct}%</span>
                </div>
                <div className="w-full bg-gray-100 rounded-full h-1.5 mb-4">
                  <div
                    className="bg-iim-blue rounded-full h-1.5 transition-all"
                    style={{ width: `${pct}%` }}
                  />
                </div>
                <div className="flex items-center gap-1 text-iim-blue text-sm font-medium group-hover:gap-2 transition-all">
                  Ver planeaciones <ChevronRight size={16} />
                </div>
              </div>
            </button>
          )
        })}
      </div>
    </div>
  )
}

// ─── Vista 2: Grid de 14 semanas ──────────────────────────────────────────────
function MateriaDetalle({ asignacion, onBack, onEditSemana }) {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)

  const cargar = useCallback(() => {
    setLoading(true)
    axios.get(`${API}/api/maestro/planeaciones/${asignacion.asignacion_id}`)
      .then(r => setData(r.data))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [asignacion.asignacion_id])

  useEffect(() => { cargar() }, [cargar])

  if (loading) return (
    <div className="flex items-center justify-center h-48 text-gray-400">
      <Loader size={24} className="animate-spin mr-2" /> Cargando semanas…
    </div>
  )

  return (
    <div>
      {/* Header */}
      <div className="flex items-center gap-3 mb-6">
        <button
          onClick={onBack}
          className="flex items-center gap-1 text-sm text-gray-500 hover:text-iim-blue transition"
        >
          <ChevronLeft size={16} /> Mis materias
        </button>
        <span className="text-gray-300">/</span>
        <span className="font-bold text-iim-blue">{data?.materia}</span>
        {data?.grupo && (
          <span className="text-xs bg-iim-blue/10 text-iim-blue px-2 py-0.5 rounded-full">
            Grupo {data.grupo}
          </span>
        )}
      </div>

      {/* Stats */}
      <div className="flex gap-4 mb-6 flex-wrap">
        {[
          {
            label: 'Semanas totales',
            val: data?.semanas || 14,
            color: 'text-iim-blue',
          },
          {
            label: 'Con planeación',
            val: data?.planeaciones.filter(p => p.contenido).length || 0,
            color: 'text-green-600',
          },
          {
            label: 'Pendientes',
            val: (data?.semanas || 14) - (data?.planeaciones.filter(p => p.contenido).length || 0),
            color: 'text-orange-500',
          },
        ].map(s => (
          <div key={s.label} className="bg-white rounded-xl shadow-sm border border-gray-100 px-5 py-3">
            <div className={`text-2xl font-bold ${s.color}`}>{s.val}</div>
            <div className="text-xs text-gray-500">{s.label}</div>
          </div>
        ))}
      </div>

      {/* Grid semanas */}
      <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-7 gap-3">
        {data?.planeaciones.map(p => {
          const tiene = Boolean(p.contenido)
          return (
            <button
              key={p.semana}
              onClick={() => onEditSemana(p, data, cargar)}
              className={`rounded-xl border p-3 text-left transition hover:shadow-md group ${
                tiene
                  ? 'bg-green-50 border-green-200 hover:border-green-400'
                  : 'bg-gray-50 border-gray-200 hover:border-iim-blue'
              }`}
            >
              <div className="text-xs font-bold text-gray-400 mb-1">SEMANA</div>
              <div className="text-2xl font-bold text-gray-700 mb-2">{p.semana}</div>
              {tiene ? (
                <>
                  <CheckCircle size={16} className="text-green-500 mb-1" />
                  <div className="text-xs text-gray-500 truncate font-medium">{p.titulo || 'Sin título'}</div>
                </>
              ) : (
                <>
                  <div className="w-4 h-4 rounded-full border-2 border-gray-300 mb-1 group-hover:border-iim-blue transition" />
                  <div className="text-xs text-gray-400">Sin planeación</div>
                </>
              )}
            </button>
          )
        })}
      </div>
    </div>
  )
}

// ─── Vista 3: Editor de planeación ────────────────────────────────────────────
function PlaneacionEditor({ semana, asignacion_id, onBack, onSaved }) {
  const [titulo, setTitulo] = useState(semana.titulo || '')
  const [contenido, setContenido] = useState(semana.contenido || '')
  const [saving, setSaving] = useState(false)
  const [ok, setOk] = useState(false)
  const [error, setError] = useState(null)

  const guardar = async () => {
    setSaving(true)
    setError(null)
    try {
      await axios.put(`${API}/api/maestro/planeaciones/${asignacion_id}/${semana.semana}`, {
        titulo,
        contenido,
      })
      setOk(true)
      setTimeout(() => {
        setOk(false)
        onSaved()
      }, 800)
    } catch {
      setError('Error al guardar. Intenta de nuevo.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="max-w-2xl">
      {/* Header */}
      <div className="flex items-center gap-3 mb-6">
        <button
          onClick={onBack}
          className="flex items-center gap-1 text-sm text-gray-500 hover:text-iim-blue transition"
        >
          <ChevronLeft size={16} /> Semanas
        </button>
        <span className="text-gray-300">/</span>
        <span className="font-bold text-iim-blue">Semana {semana.semana}</span>
      </div>

      <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
        <div className="flex items-center gap-2 mb-5">
          <FileText size={18} className="text-iim-blue" />
          <h2 className="font-bold text-iim-blue text-lg">Planeación — Semana {semana.semana}</h2>
        </div>

        {/* Título */}
        <div className="mb-4">
          <label className="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-1.5 block">
            Título del tema
          </label>
          <input
            value={titulo}
            onChange={e => setTitulo(e.target.value)}
            placeholder="Ej. Álgebra lineal — Sistemas de ecuaciones"
            className="w-full border border-gray-200 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:border-iim-blue focus:ring-1 focus:ring-iim-blue"
          />
        </div>

        {/* Contenido */}
        <div className="mb-5">
          <label className="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-1.5 block">
            Planeación / Contenido
          </label>
          <textarea
            value={contenido}
            onChange={e => setContenido(e.target.value)}
            placeholder="Describe los temas, actividades, objetivos y materiales de esta semana…"
            rows={10}
            className="w-full border border-gray-200 rounded-lg px-4 py-3 text-sm focus:outline-none focus:border-iim-blue focus:ring-1 focus:ring-iim-blue resize-y"
          />
        </div>

        {error && (
          <div className="flex items-center gap-2 text-red-500 text-sm mb-4">
            <AlertCircle size={16} /> {error}
          </div>
        )}

        <div className="flex gap-3">
          <button
            onClick={guardar}
            disabled={saving || ok}
            className={`flex items-center gap-2 px-6 py-2.5 rounded-lg font-semibold text-sm transition ${
              ok
                ? 'bg-green-500 text-white'
                : 'bg-iim-blue text-white hover:bg-iim-blue/90'
            } disabled:opacity-70`}
          >
            {saving ? <Loader size={15} className="animate-spin" /> : ok ? <CheckCircle size={15} /> : <Save size={15} />}
            {saving ? 'Guardando…' : ok ? '¡Guardado!' : 'Guardar planeación'}
          </button>
          <button
            onClick={onBack}
            className="px-5 py-2.5 text-sm text-gray-500 hover:text-gray-700 transition"
          >
            Cancelar
          </button>
        </div>
      </div>
    </div>
  )
}

// ─── Componente principal ─────────────────────────────────────────────────────
export default function MaestroDashboard() {
  // view: 'materias' | 'detalle' | 'editor'
  const [view, setView] = useState('materias')
  const [selectedMateria, setSelectedMateria] = useState(null)
  const [selectedSemana, setSelectedSemana] = useState(null)
  const [reloadDetalle, setReloadDetalle] = useState(null)

  const handleSelectMateria = (m) => {
    setSelectedMateria(m)
    setView('detalle')
  }

  const handleEditSemana = (semana, data, recargar) => {
    setSelectedSemana({ ...semana, asignacion_id: data.asignacion_id })
    setReloadDetalle(() => recargar)
    setView('editor')
  }

  const handleSaved = () => {
    setView('detalle')
    if (reloadDetalle) reloadDetalle()
  }

  return (
    <DashboardLayout title="Portal del Maestro">
      {view === 'materias' && (
        <MisMaterias onSelectMateria={handleSelectMateria} />
      )}
      {view === 'detalle' && selectedMateria && (
        <MateriaDetalle
          asignacion={selectedMateria}
          onBack={() => setView('materias')}
          onEditSemana={handleEditSemana}
        />
      )}
      {view === 'editor' && selectedSemana && (
        <PlaneacionEditor
          semana={selectedSemana}
          asignacion_id={selectedSemana.asignacion_id}
          onBack={() => setView('detalle')}
          onSaved={handleSaved}
        />
      )}
    </DashboardLayout>
  )
}
