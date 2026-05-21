<script setup>
defineProps({
  size: { type: Number, default: 64 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <circle cx="18" cy="36" r="10" fill="#8BAAB8"/>
    <circle cx="30" cy="26" r="14" fill="#8BAAB8"/>
    <circle cx="46" cy="30" r="12" fill="#8BAAB8"/>
    <rect x="8" y="36" width="50" height="14" fill="#8BAAB8"/>
  </svg>
</template>
