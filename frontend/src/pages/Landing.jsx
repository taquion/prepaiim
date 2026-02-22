import { Link } from 'react-router-dom'
import { Globe, Users, BookOpen, CreditCard, ArrowRight } from 'lucide-react'
import logo from '../assets/logo.jpg'

const InstagramIcon = ({ size = 24 }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <rect x="2" y="2" width="20" height="20" rx="5" ry="5"/>
    <path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"/>
    <line x1="17.5" y1="6.5" x2="17.51" y2="6.5"/>
  </svg>
)

export default function Landing() {
  return (
    <div className="min-h-screen bg-white">
      {/* Navbar */}
      <nav className="bg-iim-blue text-white shadow-lg sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-full bg-iim-teal flex items-center justify-center">
              <Globe size={20} className="text-white" />
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

          {/* Logo visible sobre fondo oscuro en tarjeta blanca */}
          <div className="flex justify-center mb-8">
            <div className="bg-white rounded-2xl shadow-xl px-8 py-4 inline-block">
              <img src={logo} alt="Prepa IIM" className="h-20 w-auto object-contain" />
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
            <Link
              to="/login"
              className="bg-iim-gold hover:bg-iim-teal text-white font-bold px-8 py-3 rounded-lg text-lg transition flex items-center justify-center gap-2"
            >
              Acceder al Portal <ArrowRight size={18} />
            </Link>
            <a
              href="#contacto"
              className="border-2 border-white text-white hover:bg-white hover:text-iim-blue font-bold px-8 py-3 rounded-lg text-lg transition"
            >
              Más información
            </a>
          </div>
        </div>
      </section>

      {/* Stats bar */}
      <section className="bg-iim-dark text-white py-8 px-4">
        <div className="max-w-4xl mx-auto grid grid-cols-3 gap-4 text-center">
          {[
            { value: '10+', label: 'Años de experiencia' },
            { value: '4', label: 'Roles en el portal' },
            { value: '100%', label: 'Dedicación académica' },
          ].map((s) => (
            <div key={s.label}>
              <div className="text-2xl sm:text-3xl font-extrabold text-iim-gold">{s.value}</div>
              <div className="text-xs sm:text-sm text-blue-200 mt-1">{s.label}</div>
            </div>
          ))}
        </div>
      </section>

      {/* Portal Features */}
      <section className="py-20 px-4 bg-gray-50">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-iim-blue mb-3">Portal Escolar Integrado</h2>
            <div className="w-16 h-1 bg-iim-gold mx-auto mb-4"></div>
            <p className="text-gray-500 max-w-xl mx-auto">
              Una plataforma unificada para toda la comunidad IIM
            </p>
          </div>
          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
            {[
              {
                emoji: '🏛️',
                title: 'Administrativos',
                desc: 'Gestión de pagos, adeudos, becas y vista general del plantel.',
                color: 'border-iim-blue',
              },
              {
                emoji: '👨🏫',
                title: 'Maestros',
                desc: 'Subida de contenido, tareas, exámenes y vaciado de calificaciones.',
                color: 'border-iim-teal',
              },
              {
                emoji: '🎓',
                title: 'Alumnos',
                desc: 'Avance por tetramestre, materias, calificaciones y tareas.',
                color: 'border-iim-gold',
              },
              {
                emoji: '👨👩👧',
                title: 'Padres',
                desc: 'Consulta de adeudos, pagos y calificaciones de sus hijos.',
                color: 'border-iim-light',
              },
            ].map((f) => (
              <div
                key={f.title}
                className={`bg-white rounded-xl shadow-sm border-t-4 ${f.color} p-6 hover:shadow-md transition`}
              >
                <div className="text-3xl mb-3">{f.emoji}</div>
                <h3 className="text-lg font-bold text-iim-blue mb-2">{f.title}</h3>
                <p className="text-gray-500 text-sm leading-relaxed">{f.desc}</p>
              </div>
            ))}
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
          <h2 className="text-3xl font-bold mb-4">¿Listo para ingresar?</h2>
          <p className="text-blue-100 mb-8 text-lg">
            Accede al portal escolar con tus credenciales asignadas.
          </p>
          <Link
            to="/login"
            className="bg-iim-gold hover:bg-iim-teal text-white font-bold px-10 py-3 rounded-lg text-lg transition inline-flex items-center gap-2"
          >
            Ingresar al Portal <ArrowRight size={18} />
          </Link>
        </div>
      </section>

      {/* Contact */}
      <section id="contacto" className="py-16 px-4 bg-gray-50">
        <div className="max-w-4xl mx-auto text-center">
          <h2 className="text-3xl font-bold text-iim-blue mb-4">Contacto</h2>
          <div className="w-16 h-1 bg-iim-gold mx-auto mb-8"></div>

          {/* Logo en sección de contacto (fondo blanco — se ve perfecto) */}
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
