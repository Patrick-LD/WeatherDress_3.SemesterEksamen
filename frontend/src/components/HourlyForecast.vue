<script setup>
import { computed } from 'vue'
import SolIcon from './icons/SolIcon.vue'
import RegnIcon from './icons/RegnIcon.vue'
import SkyetIcon from './icons/SkyetIcon.vue'
import SneIcon from './icons/SneIcon.vue'

const props = defineProps({
  data: { type: Array, required: true }
})

function getIcon(description) {
  const desc = description?.toLowerCase() ?? ''
  if (desc.includes('sne')) return SneIcon
  if (desc.includes('regn') || desc.includes('støvregn') || desc.includes('torden')) return RegnIcon
  if (desc.includes('skyet') || desc.includes('overskyet') || desc.includes('tåge')) return SkyetIcon
  if (desc.includes('klar') || desc.includes('sol')) return SolIcon
  return SkyetIcon
}

const upcomingHours = computed(() => {
  const now = new Date()
  const currentHour = now.getHours()
  const today = now.toISOString().substring(0, 10)
  return props.data.filter(e => {
    if (e.date > today) return true
    if (e.date === today) return parseInt(e.time.split(':')[0]) >= currentHour
    return false
  })
})
</script>

<template>
  <div class="card" id="hourly-forecast">
    <h2>Vejr de næste timer</h2>
    <div class="hourly-scroll">
      <template v-for="(hour, i) in upcomingHours" :key="hour.date + hour.time">
        <div v-if="i > 0 && hour.date !== upcomingHours[i - 1].date" class="day-divider">
          I morgen
        </div>
        <div class="hour-item" :class="{ 'hour-now': i === 0 }">
          <span class="hour-time">{{ hour.time }}</span>
          <component :is="getIcon(hour.description)" :size="32" />
          <span class="hour-temp">{{ Math.round(hour.temperatureC) }}°</span>
          <span class="hour-precip" v-if="hour.precipitation > 0">{{ hour.precipitation }} mm</span>
        </div>
      </template>
    </div>
  </div>
</template>

<style scoped>
.hourly-scroll {
  display: flex;
  gap: 16px;
  overflow-x: auto;
  padding-bottom: 8px;
  scrollbar-width: thin;
  scrollbar-color: #cde4f5 transparent;
}

.hour-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
  min-width: 64px;
  padding: 8px 4px;
  border-radius: 10px;
  flex-shrink: 0;
}

.hour-now {
  background: #eaf4fd;
}

.day-divider {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-width: 56px;
  font-size: 0.75rem;
  font-weight: 700;
  color: #1a6da8;
  border-left: 2px solid #cde4f5;
  padding: 0 8px;
  flex-shrink: 0;
}

.hour-time {
  font-size: 0.8rem;
  color: #888;
  font-weight: 600;
}

.hour-temp {
  font-size: 1rem;
  font-weight: 700;
  color: #1a1a2e;
}

.hour-precip {
  font-size: 0.75rem;
  color: #1a6da8;
}
</style>
