export function capitalize(str) {
  if (!str) return ''
  return str.charAt(0).toUpperCase() + str.slice(1)
}

export function findNuværendeTime(data) {
  if (!data.length) return {}
  const targetTime = new Date().getHours().toString().padStart(2, '0') + ':00'
  return data.find(e => e.time === targetTime) ?? data[0]
}

export function findMiddag(data) {
  if (!data.length) return {}
  return data.find(e => e.time === '12:00') ?? data[12] ?? data[0]
}
