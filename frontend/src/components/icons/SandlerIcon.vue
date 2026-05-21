<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <ellipse cx="32" cy="50" rx="24" ry="7" fill="#c8a060" stroke="#8a6030" stroke-width="2"/>
    <ellipse cx="32" cy="54" rx="24" ry="4" fill="#8a6030"/>
    <path d="M16 46 Q24 36 32 34 Q40 36 48 46" fill="none" stroke="#8a6030" stroke-width="3" stroke-linecap="round"/>
    <path d="M20 50 L28 42 M36 42 L44 50" stroke="#8a6030" stroke-width="2.5" stroke-linecap="round"/>
  </svg>
</template>
