import { useState } from 'react'
import { Link } from 'react-router-dom'
import { ArrowRight, X, CheckCircle, GraduationCap, Users, Thermometer, Shield, BadgeCheck, HandCoins } from 'lucide-react'
import logo from '../assets/logo.jpg'
import bear from '../assets/bear.jpg'
import bearWhite from '../assets/bear_white.png'
import sepBadge from '../assets/sep.jpg'
import foto1 from '../assets/foto1.jpg'
import foto2 from '../assets/foto2.jpg'
import foto3 from '../assets/foto3.jpg'
import foto4 from '../assets/foto4.jpg'
import foto5 from '../assets/foto5.jpg'

const InstagramIcon = ({ size = 24 }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <rect x="2" y="2" width="20" height="20" rx="5" ry="5"/>
    <path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"/>
    <line x1="17.5" y1="6.5" x2="17.51" y2="6.5"/>
  </svg>
)

const WhatsAppIcon = ({ size = 32 }) => (
  <svg width={size} height={size} viewBox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <circle cx="16" cy="16" r="16" fill="#25D366"/>
    <path d="M22.5 9.5A9.1 9.1 0 0 0 7.3 20.7L6 26l5.5-1.4a9.1 9.1 0 0 0 4.4 1.1A9.1 9.1 0 0 0 25 16.5 9.1 9.1 0 0 0 22.5 9.5zm-6.6 14a7.6 7.6 0 0 1-3.8-1l-.3-.2-3.2.8.9-3.1-.2-.3a7.6 7.6 0 1 1 6.6 3.8zm4.2-5.7c-.2-.1-1.4-.7-1.6-.8s-.4-.1-.5.1-.6.8-.8 1-.3.1-.5 0a6.4 6.4 0 0 1-1.9-1.2 7.1 7.1 0 0 1-1.3-1.6c-.1-.2 0-.4.1-.5l.4-.4.2-.3v-.3l-.7-1.7c-.2-.4-.4-.4-.5-.4h-.5a.9.9 0 0 0-.7.3 2.9 2.9 0 0 0-.9 2.1 5 5 0 0 0 1.1 2.7 11.5 11.5 0 0 0 4.4 3.9c.6.3 1.1.4 1.5.3a2.6 2.6 0 0 0 1.7-1.2 2.1 2.1 0 0 0 .1-1.2c0-.1-.2-.2-.5-.3z" fill="white"/>
  </svg>
)

const GoogleMapsIcon = ({ size = 24 }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
    <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z" fill="#EA4335"/>
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
                className="w-full bg-iim-teal hover:bg-iim-dark text-white font-bold py-3 rounded-lg transition disabled:opacity-60 text-sm"
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
  const [ctaName, setCtaName]     = useState('')
  const [ctaPhone, setCtaPhone]   = useState('')
  const [ctaStatus, setCtaStatus] = useState('idle') // idle | loading | ok | error

  const submitCta = async (e) => {
    e.preventDefault()
    if (!ctaName.trim() || !ctaPhone.trim()) return
    setCtaStatus('loading')
    try {
      const res = await fetch('/api/inscripciones', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ nombre: ctaName, telefono: ctaPhone, secundaria: '' }),
      })
      setCtaStatus(res.ok ? 'ok' : 'error')
    } catch { setCtaStatus('error') }
  }

  return (
    <div className="min-h-screen bg-white">
      {/* Modal */}
      {modalOpen && <InscripcionModal onClose={() => setModalOpen(false)} />}

      {/* Franja anuncio */}
      <div className="bg-orange-500 text-white text-center py-2 px-4">
        {/* Mobile: copy corto */}
        <p className="sm:hidden text-xs font-bold tracking-widest uppercase">
          🎓 ¡INSCRIPCIONES ABIERTAS!{' '}
          <button onClick={() => setModalOpen(true)} className="underline hover:text-orange-100 transition">
            Inscríbete hoy
          </button>
        </p>
        {/* Desktop: copy completo */}
        <p className="hidden sm:block text-sm font-bold tracking-widest uppercase">
          🎓 INSCRIPCIONES ABIERTAS — Cupo limitado ·{' '}
          <button onClick={() => setModalOpen(true)} className="underline hover:text-orange-100 transition">
            Inscríbete hoy
          </button>
        </p>
      </div>

      {/* Navbar */}
      <nav className="bg-iim-blue text-white shadow-lg sticky top-0 z-40">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-11 h-11 flex items-center justify-center">
              <img
                src={bearWhite}
                alt="Prepa IIM"
                className="w-full h-full object-contain"
              />
            </div>
            <div>
              <div className="text-sm sm:text-base font-bold leading-tight">Instituto Intercultural</div>
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
      <section
        className="relative text-white pt-8 sm:pt-16 pb-16 sm:pb-20 px-4 overflow-hidden"
        style={{ backgroundImage: `url(${foto3})`, backgroundSize: 'cover', backgroundPosition: 'center top' }}
      >
        {/* Overlay azul oscuro sobre la foto */}
        <div className="absolute inset-0" style={{ background: "linear-gradient(to bottom, rgba(26,46,90,0.93) 0%, rgba(27,45,96,0.87) 50%, rgba(26,46,90,0.95) 100%)" }} />
        <div className="relative z-10 max-w-5xl mx-auto text-center">

          {/* Logo más grande */}
          <div className="flex justify-center mb-8">
            <div className="bg-white rounded-2xl shadow-xl px-6 py-6 w-full max-w-sm">
              <img src={logo} alt="Prepa IIM" className="w-full h-auto object-contain block" />
            </div>
          </div>

          <h1 className="text-4xl sm:text-5xl lg:text-6xl font-extrabold mb-6 leading-tight">
            Sin educación el mundo te cierra muchas puertas,<br />
            <span className="text-iim-gold">pero nosotros te daremos la llave.</span>
          </h1>
          <p className="text-lg sm:text-xl text-blue-100 mb-2 max-w-2xl mx-auto leading-relaxed">
            Bachillerato General de 2 años, incorporado a la SEP desde hace más de 18 años.
          </p>
          <p className="text-xl sm:text-2xl font-bold text-orange-400 mb-10">
            ¡Sin examen de admisión!
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

      {/* Galería fotográfica — franja auto-scroll */}
      <section className="py-8 overflow-hidden bg-white">
        <style>{`
          @keyframes scroll-left {
            0%   { transform: translateX(0); }
            100% { transform: translateX(-50%); }
          }
          .photo-strip {
            display: flex;
            width: max-content;
            animation: scroll-left 18s linear infinite;
          }
          .photo-strip:hover {
            animation-play-state: paused;
          }
        `}</style>
        <div className="overflow-hidden">
          <div className="photo-strip">
            {[foto1, foto2, foto3, foto4, foto5, foto1, foto2, foto3, foto4, foto5].map((src, i) => (
              <div
                key={i}
                className="flex-shrink-0 mx-2 rounded-xl overflow-hidden"
                style={{ width: '280px', height: '190px' }}
              >
                <img
                  src={src}
                  alt={`Prepa IIM ${(i % 5) + 1}`}
                  className="w-full h-full object-cover"
                />
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ¿Por qué PrepaIIM? */}
      <section className="py-20 px-4 bg-gray-50">
        <div className="max-w-5xl mx-auto">
          <div className="text-center mb-14">
            <h2 className="text-3xl sm:text-4xl font-extrabold text-iim-blue mb-3">
              ¿Por qué estudiar en Prepa<span className="text-iim-teal">IIM</span>?
            </h2>
            <div className="w-16 h-1 bg-orange-500 mx-auto mb-4"></div>

          </div>
          <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-6">

            {/* 1 */}
            <div className="bg-white rounded-2xl p-6 shadow-sm hover:shadow-md border border-gray-100 hover:border-iim-teal transition-all duration-200 flex flex-col gap-4">
              <div className="w-12 h-12 rounded-xl bg-blue-50 text-iim-blue flex items-center justify-center">
                <GraduationCap size={28} />
              </div>
              <h3 className="text-lg font-bold text-iim-blue">Sin examen de admisión</h3>
              <p className="text-gray-600 text-sm leading-relaxed">No le complicamos las ganas de estudiar a nadie. Si quieres aprender, aquí tienes tu lugar.</p>
            </div>

            {/* 2 */}
            <div className="bg-white rounded-2xl p-6 shadow-sm hover:shadow-md border border-gray-100 hover:border-iim-teal transition-all duration-200 flex flex-col gap-4">
              <div className="w-12 h-12 rounded-xl bg-teal-50 text-teal-600 flex items-center justify-center">
                <Users size={28} />
              </div>
              <h3 className="text-lg font-bold text-iim-blue">Somos una prepa cercana</h3>
              <p className="text-gray-600 text-sm leading-relaxed">Aquí no eres un número más. Todos los alumnos pueden hablar con la Directora a un WhatsApp de distancia.</p>
            </div>

            {/* 3 */}
            <div className="bg-white rounded-2xl p-6 shadow-sm hover:shadow-md border border-gray-100 hover:border-iim-teal transition-all duration-200 flex flex-col gap-4">
              <div className="w-12 h-12 rounded-xl bg-sky-50 text-sky-500 flex items-center justify-center">
                <Thermometer size={28} />
              </div>
              <h3 className="text-lg font-bold text-iim-blue">Salones climatizados</h3>
              <p className="text-gray-600 text-sm leading-relaxed">Todos nuestros salones cuentan con aire acondicionado para que estudies cómodo sin importar el calor.</p>
            </div>

            {/* 4 */}
            <div className="bg-white rounded-2xl p-6 shadow-sm hover:shadow-md border border-gray-100 hover:border-iim-teal transition-all duration-200 flex flex-col gap-4">
              <div className="w-12 h-12 rounded-xl bg-green-50 text-green-600 flex items-center justify-center">
                <Shield size={28} />
              </div>
              <h3 className="text-lg font-bold text-iim-blue">Ambiente seguro</h3>
              <p className="text-gray-600 text-sm leading-relaxed">Contamos con cámaras en todas las áreas y barda perimetral para que tú y tu familia estén tranquilos.</p>
            </div>

            {/* 5 */}
            <div className="bg-white rounded-2xl p-6 shadow-sm hover:shadow-md border border-gray-100 hover:border-iim-teal transition-all duration-200 flex flex-col gap-4">
              <div className="w-12 h-12 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center">
                <BadgeCheck size={28} />
              </div>
              <h3 className="text-lg font-bold text-iim-blue">Validez oficial SEP</h3>
              <p className="text-gray-600 text-sm leading-relaxed">Al terminar puedes entrar a cualquier universidad que elijas, pública o privada, en todo el país.</p>
            </div>

            {/* 6 */}
            <div className="bg-white rounded-2xl p-6 shadow-sm hover:shadow-md border border-gray-100 hover:border-iim-teal transition-all duration-200 flex flex-col gap-4">
              <div className="w-12 h-12 rounded-xl bg-orange-50 text-orange-500 flex items-center justify-center">
                <HandCoins size={28} />
              </div>
              <h3 className="text-lg font-bold text-iim-blue">Precios accesibles y becas</h3>
              <p className="text-gray-600 text-sm leading-relaxed">¡Tenemos precios accesibles y muchas becas disponibles! La educación no debe ser un lujo.</p>
            </div>

          </div>
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
                desc: 'Nuestro compromiso con cada alumno es que logren terminar su preparatoria, en un ambiente seguro y agradable para todos.',
              },
              {
                title: 'Visión',
                desc: 'Darles a nuestros alumnos las herramientas para que puedan tener el futuro con el que ellos han soñado.',
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

      {/* CTA inline */}
      <section className="iim-gradient text-white py-16 px-4">
        <div className="max-w-xl mx-auto text-center">
          <h2 className="text-3xl font-bold mb-3">¿Listo para asegurar tu futuro?</h2>
          <p className="text-blue-100 mb-8 text-lg">
            Déjanos tus datos y te contactamos hoy mismo.
          </p>
          {ctaStatus === 'ok' ? (
            <div className="bg-white/10 rounded-2xl p-8 text-center">
              <CheckCircle size={48} className="text-green-400 mx-auto mb-3" />
              <p className="text-xl font-bold">¡Listo! Te contactamos pronto 🎉</p>
            </div>
          ) : (
            <form onSubmit={submitCta} className="bg-white/10 backdrop-blur rounded-2xl p-6 flex flex-col sm:flex-row gap-3">
              <input
                type="text"
                placeholder="Tu nombre"
                value={ctaName}
                onChange={e => setCtaName(e.target.value)}
                required
                className="flex-1 bg-white/90 text-gray-800 rounded-lg px-4 py-3 text-sm font-medium placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-orange-400"
              />
              <input
                type="tel"
                placeholder="WhatsApp / Teléfono"
                value={ctaPhone}
                onChange={e => setCtaPhone(e.target.value)}
                required
                className="flex-1 bg-white/90 text-gray-800 rounded-lg px-4 py-3 text-sm font-medium placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-orange-400"
              />
              <button
                type="submit"
                disabled={ctaStatus === 'loading'}
                className="bg-orange-500 hover:bg-orange-600 text-white font-bold px-6 py-3 rounded-lg transition disabled:opacity-60 whitespace-nowrap flex items-center gap-2"
              >
                {ctaStatus === 'loading' ? 'Enviando...' : <><span>Inscríbete</span><ArrowRight size={16} /></>}
              </button>
            </form>
          )}
          {ctaStatus === 'error' && (
            <p className="text-red-300 text-sm mt-3">Error al enviar. Escríbenos por WhatsApp.</p>
          )}
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

          <p className="text-gray-600 text-lg font-semibold">Instituto Intercultural Monterrey, Pte.</p>

          {/* Datos de contacto */}
          <div className="mt-6 flex flex-col sm:flex-row gap-4 justify-center">
            <a
              href="https://wa.me/528124489796"
              target="_blank"
              rel="noopener noreferrer"
              className="flex items-center gap-4 bg-white border border-gray-200 rounded-2xl px-6 py-4 shadow-sm hover:shadow-md hover:border-green-400 transition"
            >
              <WhatsAppIcon size={44} />
              <div className="text-left">
                <div className="text-xs text-gray-400 font-medium">WhatsApp</div>
                <div className="text-lg font-bold text-gray-800">812 448 9796</div>
              </div>
            </a>
            <a
              href="tel:8111682424"
              className="flex items-center gap-4 bg-white border border-gray-200 rounded-2xl px-6 py-4 shadow-sm hover:shadow-md hover:border-iim-teal transition"
            >
              <span className="text-4xl">📞</span>
              <div className="text-left">
                <div className="text-xs text-gray-400 font-medium">Teléfono</div>
                <div className="text-lg font-bold text-gray-800">811 168 2424</div>
              </div>
            </a>
          </div>

          {/* Dirección */}
          <div className="mt-5">
            <a
              href="https://maps.google.com/?q=Ave.+Cabezada+10907,+Col.+Barrio+Acero,+Solidaridad,+Monterrey,+NL"
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-2 bg-white border border-gray-200 rounded-2xl px-6 py-4 shadow-sm hover:shadow-md hover:border-red-300 transition"
            >
              <GoogleMapsIcon size={28} />
              <div className="text-left">
                <div className="text-xs text-gray-400 font-medium">Dirección · Ver en Google Maps</div>
                <div className="text-sm font-semibold text-gray-700">Ave. Cabezada 10907, Col. Barrio Acero, Solidaridad, Mty., NL</div>
              </div>
            </a>
          </div>

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

      {/* Botón flotante WhatsApp */}
      <a
        href="https://wa.me/528124489796"
        target="_blank"
        rel="noopener noreferrer"
        className="fixed bottom-5 right-5 z-50 bg-green-500 hover:bg-green-600 text-white rounded-full shadow-lg hover:shadow-xl transition-all duration-200 flex items-center justify-center w-14 h-14"
        title="Escríbenos por WhatsApp"
      >
        <svg viewBox="0 0 24 24" fill="currentColor" className="w-7 h-7">
          <path d="M17.472 14.382c-.297-.149-1.758-.867-2.03-.967-.273-.099-.471-.148-.67.15-.197.297-.767.966-.94 1.164-.173.199-.347.223-.644.075-.297-.15-1.255-.463-2.39-1.475-.883-.788-1.48-1.761-1.653-2.059-.173-.297-.018-.458.13-.606.134-.133.298-.347.446-.52.149-.174.198-.298.298-.497.099-.198.05-.371-.025-.52-.075-.149-.669-1.612-.916-2.207-.242-.579-.487-.5-.669-.51-.173-.008-.371-.01-.57-.01-.198 0-.52.074-.792.372-.272.297-1.04 1.016-1.04 2.479 0 1.462 1.065 2.875 1.213 3.074.149.198 2.096 3.2 5.077 4.487.709.306 1.262.489 1.694.625.712.227 1.36.195 1.871.118.571-.085 1.758-.719 2.006-1.413.248-.694.248-1.289.173-1.413-.074-.124-.272-.198-.57-.347m-5.421 7.403h-.004a9.87 9.87 0 01-5.031-1.378l-.361-.214-3.741.982.998-3.648-.235-.374a9.86 9.86 0 01-1.51-5.26c.001-5.45 4.436-9.884 9.888-9.884 2.64 0 5.122 1.03 6.988 2.898a9.825 9.825 0 012.893 6.994c-.003 5.45-4.437 9.884-9.885 9.884m8.413-18.297A11.815 11.815 0 0012.05 0C5.495 0 .16 5.335.157 11.892c0 2.096.547 4.142 1.588 5.945L.057 24l6.305-1.654a11.882 11.882 0 005.683 1.448h.005c6.554 0 11.89-5.335 11.893-11.893a11.821 11.821 0 00-3.48-8.413Z"/>
        </svg>
      </a>

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
