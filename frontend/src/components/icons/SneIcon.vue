<script setup>
defineProps({
  size: { type: Number, default: 64 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <circle cx="22" cy="24" r="10" fill="#7A9DB8"/>
    <circle cx="34" cy="18" r="13" fill="#7A9DB8"/>
    <circle cx="46" cy="24" r="10" fill="#7A9DB8"/>
    <rect x="12" y="24" width="44" height="12" fill="#7A9DB8"/>
    <g transform="translate(20, 50)">
      <line x1="0" y1="-7" x2="0" y2="7" stroke="#6BAED6" stroke-width="2.5" stroke-linecap="round"/>
      <line x1="0" y1="-7" x2="0" y2="7" stroke="#6BAED6" stroke-width="2.5" stroke-linecap="round" transform="rotate(60)"/>
      <line x1="0" y1="-7" x2="0" y2="7" stroke="#6BAED6" stroke-width="2.5" stroke-linecap="round" transform="rotate(120)"/>
    </g>
    <g transform="translate(34, 52)">
      <line x1="0" y1="-7" x2="0" y2="7" stroke="#6BAED6" stroke-width="2.5" stroke-linecap="round"/>
      <line x1="0" y1="-7" x2="0" y2="7" stroke="#6BAED6" stroke-width="2.5" stroke-linecap="round" transform="rotate(60)"/>
      <line x1="0" y1="-7" x2="0" y2="7" stroke="#6BAED6" stroke-width="2.5" stroke-linecap="round" transform="rotate(120)"/>
    </g>
    <g transform="translate(48, 50)">
      <line x1="0" y1="-7" x2="0" y2="7" stroke="#6BAED6" stroke-width="2.5" stroke-linecap="round"/>
      <line x1="0" y1="-7" x2="0" y2="7" stroke="#6BAED6" stroke-width="2.5" stroke-linecap="round" transform="rotate(60)"/>
      <line x1="0" y1="-7" x2="0" y2="7" stroke="#6BAED6" stroke-width="2.5" stroke-linecap="round" transform="rotate(120)"/>
    </g>
  </svg>
</template>
