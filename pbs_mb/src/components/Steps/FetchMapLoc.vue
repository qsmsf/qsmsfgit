<template>
  <div>
    <x-header :left-options="{showBack: false}">案发地点</x-header>
    <br>
    <baidu-map class="bm-view" @ready="handler" v-bind:center="loc" :zoom="24"
      @click="showMark" ref="bmView">
      <bm-navigation anchor="BMAP_ANCHOR_TOP_RIGHT"/>
      <bm-geolocation anchor="BMAP_ANCHOR_BOTTOM_RIGHT" :showAddressBar="true" :autoLocation="true" />
      <bm-marker v-bind:position="markPosition" :dragging="true" animation="BMAP_ANIMATION_BOUNCE"
      :label="{content: '案发地点', opts: {offset: {width: 20, height: -10}}}" ref="bmMarker"></bm-marker>
    </baidu-map>
    <x-button type="primary" @click.native="getmark" >完成标记</x-button>
  </div>
</template>

<script>
import { Group, XButton, XHeader } from 'vux'
import { SET_MAPZB } from '../../mutationTypes'

export default {
  components: {
    Group,
    XHeader,
    XButton
  },
  mounted: function () {
    if (this.$store.getters.GetterToken === '') {
      this.$router.push({path: '/'})
    }
    // this.loc = this.$store.getters.GetterMapLoc === '' ? {lng: 113.931, lat: 22.528} : JSON.parse(this.$store.getters.GetterMapLoc)
    // this.markPosition = this.$store.getters.GetterMapLoc === '' ? {lng: 113.931, lat: 22.528} : JSON.parse(this.$store.getters.GetterMapLoc)
  },
  methods: {
    showMark (e) {
      this.markPosition = e.point
    },
    getmark () {
      this.$store.commit(SET_MAPZB, JSON.stringify(this.markPosition))
      this.$router.go(-1)
    },
    handler () {
      this.loc = this.$store.getters.GetterMapLoc === '' ? {lng: 113.931, lat: 22.528} : JSON.parse(this.$store.getters.GetterMapLoc)
      this.markPosition = this.$store.getters.GetterMapLoc === '' ? {lng: 113.931, lat: 22.528} : JSON.parse(this.$store.getters.GetterMapLoc)
    }
  },
  data () {
    return {
      recList: [],
      loc: {lng: 113.931, lat: 22.528},
      markPosition: {lng: 113.931, lat: 22.528}
    }
  }
}
</script>
<style>
  .bm-view {
    max-width: 98%;
    height: 400px;
  }
</style>
