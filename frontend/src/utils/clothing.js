export function beregnTøj(e) {
  if (!e || e.temperatureC === undefined) return []
  const { temperatureC: temp, windSpeed: vind, precipitation: nedbor } = e
  const items = []

  if (temp < 0)       items.push('Tyk vinterjakke', 'Hue og handsker', 'Vinterstøvler')
  else if (temp < 5)  items.push('Varm jakke', 'Hue og handsker')
  else if (temp < 10) items.push('Jakke', 'Sweater')
  else if (temp < 15) items.push('Let jakke')
  else if (temp < 20) items.push('Langærmet trøje')
  else                items.push('T-shirt')

  if (nedbor > 0) items.push('Regnjakke / paraply')
  if (vind > 10)  items.push('Vindjakke')

  return items
}
