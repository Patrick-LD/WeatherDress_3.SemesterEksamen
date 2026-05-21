export function beregnTøj(e, alleTimer = []) {
  if (!e || e.temperatureC === undefined) return []
  const { temperatureC: temp, windSpeed: vind, precipitation: nedbor } = e
  const items = []

  if (temp < 0)       items.push('Tyk vinterjakke', 'Hue og handsker')
  else if (temp < 5)  items.push('Varm jakke', 'Hue og handsker')
  else if (temp < 10) items.push('Jakke', 'Sweater')
  else if (temp < 15) items.push('Let jakke')
  else if (temp < 20) items.push('Langærmet trøje')
  else                items.push('T-shirt')

  const nuværendeTime = e.time ?? '00:00'
  const regnerSenereDag = alleTimer.some(t => t.time >= nuværendeTime && t.precipitation > 0)

  if (nedbor > 0 || regnerSenereDag) items.push('Regnjakke / paraply')
  if (vind > 10)                     items.push('Vindjakke')

  if (temp < 5)                       items.push('Vinterstøvler')
  else if (nedbor > 0 || regnerSenereDag) items.push('Gummistøvler')
  else if (temp >= 20)                items.push('Sandaler')
  else                                items.push('Sneakers')

  return items
}
