export async function hentData(url) {
  const res = await fetch(url)
  if (!res.ok) {
    const tekst = await res.text()
    throw new Error(tekst || `Fejl fra server (${res.status})`)
  }
  return res.json()
}

export function hentVejrToday(zip) {
  return hentData(`/api/weatherforecast/${zip}/today`)
}

export function hentVejrYesterday(zip) {
  return hentData(`/api/weatherforecast/${zip}/yesterday`)
}
