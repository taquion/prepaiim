/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        iim: {
          blue: '#2B437F',   // azul marino del oso
          gold: '#45A2C3',   // azul cyan del círculo del oso
          teal: '#45A2C3',   // mismo cyan
          dark: '#1B2D60',   // navy más oscuro (footer/fondos)
          light: '#5DB8D4',  // cyan más claro
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
