<template>
  <div>
    <x-header :left-options="{showBack: true, backText: '返回'}">查看最近7天的记录</x-header>
    <br>
    <scroller lock-x scrollbar-y use-pulldown  @on-pulldown-loading="loadList" ref="recordScroller" v-model="status" height="-46">
      <!--content slot-->
      <div class="box2">
        <group v-for="rec in recList">
          <Cell v-bind:title='trimDate(rec.bj_time)'  v-bind:inline-desc='rec.record_no+ "--" +rec.xz' @click.native="onChooseRec(rec)" is-link></Cell>
        </group>
      </div>

      <!--pulldown slot-->
      <div slot="pulldown" class="xs-plugin-pulldown-container xs-plugin-pulldown-down" style="position: absolute; width: 100%; height: 60px; line-height: 60px; top: -60px; text-align: center;">
        <span v-show="status.pulldownStatus === 'default'"></span>
        <span class="pulldown-arrow" v-show="status.pulldownStatus === 'down' || status.pulldownStatus === 'up'" :class="{'rotate': status.pulldownStatus === 'up'}">↓</span>
        <span v-show="status.pulldownStatus === 'loading'"><spinner type="ios-small"></spinner></span>
      </div>
    </scroller>
  </div>
</template>

<script>
import { Group, Spinner, XButton, Scroller, Cell, XHeader } from 'vux'
import { SET_RECORDBASE, SET_RECORDDISP, SET_RECORDPERSON, SET_RECORDOTHER, SET_RECORDFILES, SET_LOC, SET_QMBASE64 } from '../mutationTypes'

export default {
  components: {
    Group,
    Spinner,
    Scroller,
    Cell,
    XHeader,
    XButton
  },
  mounted: function () {
    if (this.$store.getters.GetterToken === '') {
      this.$router.push({path: '/'})
    }
    this.loadList()
    // this.kyksTime = '2017-11-13'
  },
  methods: {
    loadList () {
      this.$http({
        url: this.$store.getters.GetterBaseUrl + 'api/Records/GetRecordList',
        method: 'GET',
        emulateJSON: true,
        params: {
          userId: this.$store.getters.GetterMe.member.user_id,
          bgState: 0,
          sdate: '',
          edate: ''
        },
        headers: {
          contentType: 'application/x-www-form-urlencoded',
          token: this.$store.getters.GetterToken
        }
      }).then(function (res) {
        if (res.data.code === 100) {
          this.recList = res.data.resultList
          this.$refs.recordScroller.donePulldown()
        } else {
          this.$vux.toast.show({
            text: res.data.message,
            type: 'warn'
          })
          this.$refs.recordScroller.donePulldown()
        }
      }).catch(err => {
        this.$vux.toast.show({
          text: err,
          type: 'warn'
        })
        this.$refs.recordScroller.donePulldown()
      })
    },
    showName (i) {
      return i + ''
    },
    trimDate (strDate) {
      return strDate.substring(0, 16).replace('T', ' ')
    },
    onChooseRec (rec) {
      let entity1 = {
        record_ky_no: rec.record_ky_no,
        record_jj_no: rec.record_jj_no,
        record_aj_no: rec.record_aj_no,
        ky_unit: rec.ky_unit,
        jjr: rec.jjr,
        jjr_other: rec.jjr_other,
        bg_unit: rec.bg_unit,
        ky_date: rec.ky_date === null ? '' : rec.ky_date.substring(0, 10),
        af_time: rec.af_time === null ? '' : rec.af_time.substring(0, 16).replace('T', ' '),
        bj_time: rec.bj_time === null ? '' : rec.bj_time.substring(0, 16).replace('T', ' '),
        bjr: rec.bjr,
        bjr_sex: rec.bjr_sex,
        bjr_idcard: rec.bjr_idcard,
        bjr_phoneNo: rec.bjr_phoneNo,
        kyks_time: rec.kyks_time === null ? '' : rec.kyks_time.substring(0, 16).replace('T', ' '),
        kyjs_time: rec.kyjs_time === null ? '' : rec.kyjs_time.substring(0, 16).replace('T', ' '),
        dest_unit: rec.dest_unit,
        xz: rec.xz,
        xct_src: this.$store.getters.GetterBaseUrl + 'api/GetFile?fileName=' + rec.uuid + '_xct.jpeg&uuid='+rec.uuid,
        pmt_src: this.$store.getters.GetterBaseUrl + 'api/GetFile?fileName=' + rec.uuid + '_pmt.jpeg&uuid='+rec.uuid,
        fs_loc: rec.fs_loc,
        xc_loc: rec.xc_loc,
        xc_locpt: rec.xc_locpt,
        weather_info: rec.weather_info,
        trend_info: rec.trend_info,
        temper_info: rec.temper_info,
        humidity_info: rec.humidity_info,
        light_info: rec.light_info,
        bh_flag: rec.bh_flag,
        bhr: rec.bhr,
        bhr_name: rec.bhr_name,
        bhr_unit: rec.bhr_unit,
        bhr_unit_name: rec.bhr_unit_name,
        bhr_pos: rec.bhr_pos,
        bh_function: rec.bh_function,
        xc_info: rec.xc_info,
        bd_reason: rec.bd_reason
      }
      this.$store.commit(SET_RECORDBASE, entity1)

      let entity2 = {
        record_reason: rec.record_reason,
        xc_disp: rec.xc_disp
      }
      this.$store.commit(SET_RECORDDISP, entity2)

      let entity4 = {
        jzr: rec.jzr,
        jzr_sex: rec.jzr_sex,
        jzr_birth: rec.jzr_birth,
        jzr_address: rec.jzr_address,
        zhr: rec.zhr,
        zhr_unit: rec.zhr_unit,
        zhr_pos: rec.zhr_pos,
        blr: rec.blr,
        ztr: rec.ztr,
        zxr: rec.zxr,
        lxr: rec.lxr,
        lyr: rec.lyr,
        zhr_name: rec.zhr_name,
        zhr_unit_name: rec.zhr_unit_name,
        blr_name: rec.blr_name,
        ztr_name: rec.ztr_name,
        zxr_name: rec.zxr_name,
        lxr_name: rec.lxr_name,
        lyr_name: rec.lyr_name
      }
      this.$store.commit(SET_RECORDPERSON, entity4)

      let entity5 = {
        creater_id: rec.creater_id,
        uuid: rec.uuid,
        record_no: rec.record_no,
        record_id: rec.record_id
      }
      this.$store.commit(SET_RECORDOTHER, entity5)
      let loc = {
        east: rec.east,
        west: rec.west,
        north: rec.north,
        south: rec.south
      }
      this.$store.commit(SET_LOC, loc)
      this.$store.commit(SET_QMBASE64, '')
      this.$store.commit(SET_RECORDFILES, [])

      this.$router.push({name: 'Step1', query: {isAdd: '0', curStep: 1}})
    }
  },
  data () {
    return {
      recList: [],
      status: {
        pulldownStatus: 'default'
      }
    }
  }
}
</script>

<style lang="less" scoped>
  .box2-wrap {
    height: auto;
    overflow: hidden;
  }
  .rotate {
    transform: rotate(-180deg);
  }
  .pulldown-arrow {
    display: inline-block;
    transition: all linear 0.2s;
    color: #666;
    font-size: 25px;
  }
</style>
