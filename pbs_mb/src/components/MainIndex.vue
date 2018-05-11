<template>
  <div>
    <div class="vux-demo">
      <img class="logo" src="../assets/gongan.png">
      <h1>深圳E勘查</h1>
      <h2>深圳市南山公安分局</h2>
      <h2>{{myName}}</h2>
    </div>
    <grid>
      <grid-item label="录入案件" @click.native="onAddRec()">
        <img slot="icon" src="../assets/date_add.png">
      </grid-item>
      <grid-item label="查询案件" link="/RecordList">
        <img slot="icon" src="../assets/date_magnify.png">
      </grid-item>
      <grid-item label="test" link="/Test">
        <img slot="icon" src="../assets/date_magnify.png">
      </grid-item>
      <grid-item label="退出登录" link="/">
        <img slot="icon" src="../assets/status_offline.png">
      </grid-item>
    </grid>
  </div>
</template>

<script>
import { Group, XInput, XButton, Datetime, Grid, GridItem } from 'vux'
import { CLR_RECORDINFO } from '../mutationTypes'
export default {
  components: {
    Group,
    XInput,
    Datetime,
    Grid,
    GridItem,
    XButton
  },
  computed: {
    myName: function () {
      return this.$store.getters.GetterMe.member.user_fullname
    }
  },
  mounted: function () {
    if (this.$store.getters.GetterToken === '') {
      this.$router.push({path: '/'})
    }
  },
  methods: {
    onAddRec () {
      let date = new Date()
      let seperator1 = '-'
      let seperator2 = ':'
      let month = date.getMonth() + 1
      let strDate = date.getDate()
      let hour = date.getHours()
      let minitue = date.getMinutes()
      if (month >= 1 && month <= 9) {
        month = '0' + month
      }
      if (strDate >= 0 && strDate <= 9) {
        strDate = '0' + strDate
      }
      if (hour >= 0 && hour <= 9) {
        hour = '0' + hour
      }
      if (minitue >= 0 && minitue <= 9) {
        minitue = '0' + minitue
      }
      let curDate = date.getFullYear() + seperator1 + month + seperator1 + strDate
      let curTime = curDate + ' ' + hour + seperator2 + minitue
      let entity = {
        ky_unit: this.$store.getters.GetterMe.member.unit_id,
        bg_unit: this.$store.getters.GetterMe.member.unit_id,
        ky_date: curDate,
        jjr: this.$store.getters.GetterMe.member.user_id,
        fs_loc: '',
        af_time: curDate + ' 00:00',
        bj_time: curDate + ' 00:00',
        kyks_time: curTime,
        weather_info: '晴',
        trend_info: '',
        light_info: '自然光',
        temper_info: '20 ℃',
        humidity_info: '50%',
        bhr: this.$store.getters.GetterMe.member.user_id,
        bhr_name: this.$store.getters.GetterMe.member.user_fullname,
        bhr_unit: this.$store.getters.GetterMe.member.unit_id,
        bhr_unit_name: this.$store.getters.GetterMe.member.unit_name,
        bhr_pos: this.$store.getters.GetterMe.member.user_zw,
        bh_function: '专人看护现场，防止他人进入',
        xc_info: '原始现场',
        bd_reason: '',
        jzr: '',
        jzr_sex: '',
        jzr_birth: '',
        jzr_address: '',
        zhr: this.$store.getters.GetterMe.member.user_id,
        zhr_name: this.$store.getters.GetterMe.member.user_fullname,
        zhr_unit: this.$store.getters.GetterMe.member.unit_id,
        zhr_unit_name: this.$store.getters.GetterMe.member.unit_name,
        zhr_pos: this.$store.getters.GetterMe.member.user_zw,
        blr: this.$store.getters.GetterMe.member.user_id,
        ztr: this.$store.getters.GetterMe.member.user_id,
        zxr: this.$store.getters.GetterMe.member.user_id,
        lxr: this.$store.getters.GetterMe.member.user_id,
        blr_name: this.$store.getters.GetterMe.member.user_fullname,
        ztr_name: this.$store.getters.GetterMe.member.user_fullname,
        zxr_name: this.$store.getters.GetterMe.member.user_fullname,
        lxr_name: this.$store.getters.GetterMe.member.user_fullname,
        lyr_name: '无'
      }
      this.$store.commit(CLR_RECORDINFO, entity)
      this.$router.push({name: 'Step1', query: {isAdd: '1', curStep: 1}})
    }
  },
  data () {
    return {
    }
  }
}
</script>

<style>
</style>
