import { capitalize } from './format.js'

export function lavMeddelelse(e) {
  if (!e || !e.location) return ''
  let msg = `Det er ${capitalize(e.description)} i ${e.location} med ${Math.round(e.temperatureC)}°C. `

  if (e.windSpeed > 15)     msg += 'Der er kraftig vind — hold godt fast i hatten. '
  else if (e.windSpeed > 8) msg += 'Der er en del vind. '

  if (e.precipitation > 5)      msg += 'Det regner kraftigt — tag din paraply med!'
  else if (e.precipitation > 0) msg += 'Der er let regn.'
  else                          msg += 'Nyd din dag!'

  return msg
}
