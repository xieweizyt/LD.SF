export function parseTime(value?: string | number | Date) {
  if (!value) return '-'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return '-'
  const pad = (n: number) => n.toString().padStart(2, '0')
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} ${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`
}

export function parsePhones(text: string) {
  return text
    .split(/\r?\n|,|;|\s+/)
    .map((x) => x.trim())
    .filter(Boolean)
}

