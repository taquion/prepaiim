/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        iim: {
          blue: '#2B4480',   // azul marino principal (logo)
          gold: '#5BBAD5',   // azul claro / cyan (reemplaza dorado como acento)
          teal: '#4A9DC0',   // azul medio (variación del acento)
          dark: '#1A2E5A',   // marino oscuro (footer / fondos oscuros)
          light: '#5BBAD5',  // azul claro (alias)
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
