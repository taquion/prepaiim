/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        iim: {
          blue: '#2B4480',   // azul marino del oso
          gold: '#5BBAD5',   // azul cyan del círculo del oso (match logo)
          teal: '#5BBAD5',   // mismo cyan del logo
          dark: '#1B2D60',   // navy más oscuro (footer/fondos)
          light: '#7ECDE0',  // cyan más claro
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
