const API_BASE = import.meta.env.VITE_API_BASE ?? ''

export async function hentData(url) {
  const res = await fetch(url)
  if (!res.ok) {
    const tekst = await res.text()
    throw new Error(tekst || `Fejl fra server (${res.status})`)
  }
  return res.json()
}

export function hentVejrToday(zip) {
  return hentData(`${API_BASE}/api/weatherforecast/${zip}/today`)
}

export function hentVejrYesterday(zip) {
  return hentData(`${API_BASE}/api/weatherforecast/${zip}/yesterday`)
}

export async function triggerMotorer() {
  const res = await fetch(`${API_BASE}/api/motor/trigger`, { method: 'POST' })
  if (!res.ok) {
    const tekst = await res.text()
    throw new Error(tekst || `Fejl fra server (${res.status})`)
  }
  return res.json()
}

export function hentAnbefalingHistorik(zip) {
  return hentData(`${API_BASE}/api/weatherforecast/${zip}/recommendation-history`)
}

export function gemDagsAnbefaling(zip) {
  return hentData(`${API_BASE}/api/weatherforecast/${zip}/clothing-position`)
}
