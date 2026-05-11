<script setup>
defineProps({
  size: { type: Number, default: 64 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <line x1="32" y1="4"  x2="32" y2="14" stroke="#FFD93D" stroke-width="4" stroke-linecap="round"/>
    <line x1="32" y1="50" x2="32" y2="60" stroke="#FFD93D" stroke-width="4" stroke-linecap="round"/>
    <line x1="4"  y1="32" x2="14" y2="32" stroke="#FFD93D" stroke-width="4" stroke-linecap="round"/>
    <line x1="50" y1="32" x2="60" y2="32" stroke="#FFD93D" stroke-width="4" stroke-linecap="round"/>
    <line x1="12" y1="12" x2="19" y2="19" stroke="#FFD93D" stroke-width="4" stroke-linecap="round"/>
    <line x1="45" y1="45" x2="52" y2="52" stroke="#FFD93D" stroke-width="4" stroke-linecap="round"/>
    <line x1="52" y1="12" x2="45" y2="19" stroke="#FFD93D" stroke-width="4" stroke-linecap="round"/>
    <line x1="12" y1="52" x2="19" y2="45" stroke="#FFD93D" stroke-width="4" stroke-linecap="round"/>
    <circle cx="32" cy="32" r="13" fill="#FFD93D"/>
  </svg>
</template>
