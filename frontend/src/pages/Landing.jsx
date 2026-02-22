import { useState } from 'react'
import { Link } from 'react-router-dom'
import { ArrowRight, X, CheckCircle } from 'lucide-react'
import logo from '../assets/logo.jpg'
import bear from '../assets/bear.jpg'
import sepBadge from '../assets/sep.jpg'

const InstagramIcon = ({ size = 24 }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <rect x="2" y="2" width="20" height="20" rx="5" ry="5"/>
    <path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"/>
    <line x1="17.5" y1="6.5" x2="17.51" y2="6.5"/>
  </svg>
)

function InscripcionModal({ onClose }) {
  const [form, setForm] = useState({ nombre: '', telefono: '', secundaria: '' })
  const [loading, setLoading] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError('')
    try {
      const res = await fetch('/api/inscripciones', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(form),
      })
      if (!res.ok) throw new Error('Error al enviar')
      setSuccess(true)
    } catch {
      setError('Hubo un problema. Intenta de nuevo.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center px-4" onClick={onClose}>
      <div className="absolute inset-0 bg-black/60 backdrop-blur-sm" />
      <div
        className="relative bg-white rounded-2xl shadow-2xl w-full max-w-md p-8"
        onClick={e => e.stopPropagation()}
      >
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-400 hover:text-gray-600 transition"
        >
          <X size={20} />
        </button>

        {success ? (
          <div className="text-center py-6">
            <CheckCircle size={56} className="text-iim-teal mx-auto mb-4" />
            <h3 className="text-xl font-bold text-iim-blue mb-2">¡Listo!</h3>
            <p className="text-gray-600 mb-6">Recibimos tus datos. Pronto nos pondremos en contacto contigo.</p>
            <button
              onClick={onClose}
              className="bg-iim-blue text-white px-6 py-2 rounded-lg hover:bg-iim-dark transition"
            >
              Cerrar
            </button>
          </div>
        ) : (
          <>
            <div className="text-center mb-6">
              <img src={logo} alt="Prepa IIM" className="h-12 w-auto object-contain mx-auto mb-3" />
              <h2 className="text-2xl font-bold text-iim-blue">Inscríbete</h2>
              <p className="text-gray-500 text-sm mt-1">Déjanos tus datos y te contactamos</p>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Nombre completo</label>
                <input
                  type="text"
                  required
                  placeholder="Tu nombre"
                  value={form.nombre}
                  onChange={e => setForm({ ...form, nombre: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-iim-teal focus:border-transparent"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Teléfono</label>
                <input
                  type="tel"
                  required
                  placeholder="81 1234 5678"
                  value={form.telefono}
                  onChange={e => setForm({ ...form, telefono: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-iim-teal focus:border-transparent"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Secundaria de procedencia</label>
                <input
                  type="text"
                  required
                  placeholder="Nombre de tu secundaria"
                  value={form.secundaria}
                  onChange={e => setForm({ ...form, secundaria: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-iim-teal focus:border-transparent"
                />
              </div>

              {error && <p className="text-red-500 text-sm text-center">{error}</p>}

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-iim-blue hover:bg-iim-dark text-white font-bold py-3 rounded-lg transition disabled:opacity-60 text-sm"
              >
                {loading ? 'Enviando...' : 'Enviar mis datos'}
              </button>
            </form>
          </>
        )}
      </div>
    </div>
  )
}

export default function Landing() {
  const [modalOpen, setModalOpen] = useState(false)

  return (
    <div className="min-h-screen bg-white">
      {/* Modal */}
      {modalOpen && <InscripcionModal onClose={() => setModalOpen(false)} />}

      {/* Navbar */}
      <nav className="bg-iim-blue text-white shadow-lg sticky top-0 z-40">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-full bg-iim-teal flex items-center justify-center overflow-hidden">
              <img
                src={bear}
                alt="Prepa IIM"
                className="w-full h-full object-cover"
                style={{ mixBlendMode: 'multiply' }}
              />
            </div>
            <div>
              <div className="text-lg font-bold leading-tight">Instituto Intercultural</div>
              <div className="text-xs text-iim-gold font-medium">Monterrey, Pte.</div>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <a
              href="https://www.instagram.com/prepa_iim/"
              target="_blank"
              rel="noopener noreferrer"
              className="text-white/80 hover:text-iim-gold transition hidden sm:flex items-center gap-1.5 text-sm"
              title="Síguenos en Instagram"
            >
              <InstagramIcon size={18} />
              <span className="hidden md:inline">@prepa_iim</span>
            </a>
            <Link
              to="/login"
              className="bg-iim-gold hover:bg-iim-teal text-white font-semibold px-5 py-2 rounded-lg transition flex items-center gap-2 text-sm"
            >
              Portal Escolar <ArrowRight size={14} />
            </Link>
          </div>
        </div>
      </nav>

      {/* Hero */}
      <section className="iim-gradient text-white py-20 px-4">
        <div className="max-w-5xl mx-auto text-center">

          {/* Logo más grande */}
          <div className="flex justify-center mb-8">
            <div className="bg-white rounded-2xl shadow-xl px-10 py-6 inline-block">
              <img src={logo} alt="Prepa IIM" className="h-28 w-auto object-contain" />
            </div>
          </div>

          <h1 className="text-4xl sm:text-5xl lg:text-6xl font-extrabold mb-6 leading-tight">
            Sin educación el mundo te cierra muchas puertas,<br />
            <span className="text-iim-gold">pero nosotros te daremos la llave.</span>
          </h1>
          <p className="text-lg sm:text-xl text-blue-100 mb-10 max-w-2xl mx-auto leading-relaxed">
            Bachillerato General de 2 años, incorporado a la SEP desde hace más de 18 años.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <button
              onClick={() => setModalOpen(true)}
              className="bg-iim-gold hover:bg-iim-teal text-white font-bold px-8 py-3 rounded-lg text-lg transition flex items-center justify-center gap-2"
            >
              Inscríbete y asegura tu futuro <ArrowRight size={18} />
            </button>
            <a
              href="#contacto"
              className="border-2 border-white text-white hover:bg-white hover:text-iim-blue font-bold px-8 py-3 rounded-lg text-lg transition"
            >
              Más información
            </a>
          </div>
        </div>
      </section>

      {/* Aval SEP */}
      <section className="bg-white py-10 px-4 border-b border-gray-100">
        <div className="max-w-md mx-auto text-center">
          <img
            src={sepBadge}
            alt="Incorporado a la SEP — Autorización RVOE MCBG-010/2022"
            className="w-full h-auto object-contain mx-auto"
          />
        </div>
      </section>

      {/* Values */}
      <section className="py-20 px-4">
        <div className="max-w-5xl mx-auto text-center">
          <h2 className="text-3xl font-bold text-iim-blue mb-3">Nuestros Valores</h2>
          <div className="w-16 h-1 bg-iim-gold mx-auto mb-12"></div>
          <div className="grid md:grid-cols-3 gap-8">
            {[
              {
                title: 'Excelencia Académica',
                desc: 'Compromiso con la calidad educativa y el desarrollo integral de cada alumno.',
              },
              {
                title: 'Visión Intercultural',
                desc: 'Formación con perspectiva global y profundo respeto a la diversidad cultural.',
              },
              {
                title: 'Comunidad',
                desc: 'Una familia IIM unida en el aprendizaje, crecimiento y servicio mutuo.',
              },
            ].map((v) => (
              <div key={v.title} className="border-t-4 border-iim-gold pt-6 text-left">
                <h3 className="text-xl font-bold text-iim-blue mb-3">{v.title}</h3>
                <p className="text-gray-600 leading-relaxed">{v.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="iim-gradient text-white py-16 px-4">
        <div className="max-w-3xl mx-auto text-center">
          <h2 className="text-3xl font-bold mb-4">¿Listo para asegurar tu futuro?</h2>
          <p className="text-blue-100 mb-8 text-lg">
            Déjanos tus datos y te contactamos a la brevedad.
          </p>
          <button
            onClick={() => setModalOpen(true)}
            className="bg-iim-gold hover:bg-iim-teal text-white font-bold px-10 py-3 rounded-lg text-lg transition inline-flex items-center gap-2"
          >
            Inscríbete ahora <ArrowRight size={18} />
          </button>
        </div>
      </section>

      {/* Contact */}
      <section id="contacto" className="py-16 px-4 bg-gray-50">
        <div className="max-w-4xl mx-auto text-center">
          <h2 className="text-3xl font-bold text-iim-blue mb-4">Contacto</h2>
          <div className="w-16 h-1 bg-iim-gold mx-auto mb-8"></div>

          <div className="flex justify-center mb-6">
            <img src={logo} alt="Prepa IIM" className="h-16 w-auto object-contain" />
          </div>

          <p className="text-gray-600 text-lg">Instituto Intercultural Monterrey, Pte.</p>
          <p className="text-gray-500">Monterrey, Nuevo León, México</p>

          {/* Redes sociales */}
          <div className="mt-10">
            <h3 className="text-lg font-semibold text-iim-blue mb-5">Síguenos en redes</h3>
            <div className="flex justify-center">
              <a
                href="https://www.instagram.com/prepa_iim/"
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center gap-3 bg-white border border-gray-200 rounded-xl px-6 py-4 shadow-sm hover:shadow-md hover:border-iim-gold transition group"
              >
                <span className="text-pink-500 group-hover:scale-110 transition-transform">
                  <InstagramIcon size={28} />
                </span>
                <div className="text-left">
                  <div className="text-sm font-bold text-gray-800">@prepa_iim</div>
                  <div className="text-xs text-gray-400">Instagram</div>
                </div>
              </a>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-iim-dark text-blue-200 py-6 text-center text-sm">
        <div className="flex items-center justify-center gap-4 mb-2">
          <a
            href="https://www.instagram.com/prepa_iim/"
            target="_blank"
            rel="noopener noreferrer"
            className="text-blue-300 hover:text-iim-gold transition"
            title="Instagram @prepa_iim"
          >
            <InstagramIcon size={18} />
          </a>
        </div>
        <p>© {new Date().getFullYear()} Instituto Intercultural Monterrey — Todos los derechos reservados</p>
      </footer>
    </div>
  )
}
