/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        iim: {
          blue: '#1B4F8A',
          gold: '#C8A84B',
          teal: '#2A9D8F',
          dark: '#163D6B',
          light: '#2563B0',
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
