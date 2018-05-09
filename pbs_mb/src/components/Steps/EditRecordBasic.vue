<template>
  <div style="width: 95%;margin: 0 auto;height:100%;">
    <div style="width:100%;position:absolute;left:0;top:0;z-index:100;background-color:white">
    <x-header
      title="案件录入" :left-options="{showBack: false}"
      :right-options="{showMore: false, backText: '提交'}"
      @on-click-title="scrollTop" @on-click-more="showMenus = true">
      <span slot="right" @click="show2 = true">返回主页 |</span><span slot="right" @click="show = true">           提交  </span>
    </x-header>
    <div style="padding: 5px 5px;">
        <button-tab v-model="btnStep">
          <button-tab-item :selected="true" @click.native="goStep(1)">基本</button-tab-item>
          <button-tab-item :selected="false" @click.native="goStep(2)">描述</button-tab-item>
          <button-tab-item :selected="false" @click.native="goStep(3)">相片</button-tab-item>
          <button-tab-item :selected="step4Selected" @click.native="goStep(4)">人员</button-tab-item>
        </button-tab>
      </div>
    </div>
    <view-box ref="viewBox" body-padding-top="86px" body-padding-bottom="55px" >
      <transition name="fade">
          <router-view class="view" ref="chiledContent" ></router-view>
      </transition>
    </view-box>

      <confirm v-model="show" title="提交" @on-confirm="onConfirm" >
        <p style="text-align:center;">确认提交记录?</p>
      </confirm>
      <confirm v-model="show2" title="返回主页" @on-confirm="onHide" >
        <p style="text-align:center;">放弃本次编辑,并回到主页?</p>
      </confirm>
      <alert v-model="showSuccess" title="提交成功" @on-hide="onHide">
        <h2 style="text-align:center;">案件编号:{{recordNo}}</h2>
      </alert>
    </Group>
  </div>
</template>

<script>
import { Group, XInput, XButton, ButtonTab, ButtonTabItem, XHr, FlexboxItem, Confirm, Alert, XHeader, Tabbar, TabbarItem, ViewBox, Loading } from 'vux'
import { SET_RECORDFILES, SET_RECORDBASE, SET_RECORDDISP, SET_RECORDPERSON, SET_RECORDOTHER, SET_LOC } from '../../mutationTypes'
var uuid = require('node-uuid')

export default {
  components: {
    Group,
    XButton,
    XInput,
    ButtonTab,
    XHr,
    ButtonTabItem,
    Confirm,
    Alert,
    XHeader,
    Tabbar,
    TabbarItem,
    ViewBox,
    Loading,
    FlexboxItem
  },
  mounted: function () {
    this.isAdd = this.$route.query.isAdd
    if (this.$store.getters.GetterToken === '') {
      this.$router.push({path: '/'})
    }
    
    if (this.$route.query.curStep !== 1) {
      this.currentStep = this.$route.query.curStep
      this.btnStep = this.currentStep - 1
      this.step4Selected = true
      this.$router.push({name: 'Step' + this.$route.query.curStep, query: {isAdd: this.isAdd}})
    }
    if (this.isAdd === '1') {
      if (this.$store.getters.GetterEntity.uuid === '') {
        let entity = {
          uuid: uuid.v4(),
          creater_id: this.$store.getters.GetterMe.member.user_id,
          record_id: 0
        }
        this.$store.commit(SET_RECORDOTHER, entity)
      }
    }
  },
  methods: {
    scrollTop () {
      this.$refs.viewBox.scrollTo(0)
    },
    onConfirm () {
      this.saveCurrentStep(this.currentStep)
      if (this.isAdd === '1') {
        
        let data = JSON.stringify(this.$store.getters.GetterRecord)
        console.log(data)
        this.$http({
          url: this.$store.getters.GetterBaseUrl + 'api/Records/AddRecordDetail',
          method: 'POST',
          emulateJSON: true,
          body: {
            jsonBean: data
          },
          headers: {
            contentType: 'application/x-www-form-urlencoded',
            token: this.$store.getters.GetterToken
          }
        }).then(function (res) {
          console.log(res)
          if (res.data.code === 100) {
            this.recordNo = res.data.recNo
            this.success()
          } else {
            this.$vux.toast.show({
              text: res.data.message,
              type: 'warn'
            })
            this.disableSub = false
          }
        }).catch(err => {
          console.log(err)
          this.$vux.toast.show({
            text: err,
            type: 'warn'
          })
          this.disableSub = false
        })
      } else {
        let data = JSON.stringify(this.$store.getters.GetterRecord)
        this.$http({
          url: this.$store.getters.GetterBaseUrl + 'api/Records/UpdateRecordDetail',
          method: 'PUT',
          emulateJSON: true,
          body: {
            jsonBean: data
          },
          headers: {
            contentType: 'application/x-www-form-urlencoded',
            token: this.$store.getters.GetterToken
          }
        }).then(function (res) {
          if (res.data.code === 100) {
            this.recordNo = res.data.recNo
            this.success()
          } else {
            this.$vux.toast.show({
              text: res.data.message,
              type: 'warn'
            })
            this.disableSub = false
          }
        }).catch(err => {
          this.$vux.toast.show({
            text: err,
            type: 'warn'
          })
          this.disableSub = false
        })
      }
    },
    success () {
      this.showSuccess = true
    },
    onHide () {
      this.$router.push('/MainIndex')
      this.currentStep = 1
    },
    saveCurrentStep: function (n) {
      switch (n) {
        case 1:
          let lightInfo = ''
          if (this.$refs.chiledContent.lightList.length !== 0) {
            this.$refs.chiledContent.lightList.forEach(function (item) {
              lightInfo += item + '+'
            })
            lightInfo = lightInfo.substring(0, lightInfo.length - 1)
          }
          let entity = {
            ky_unit: this.$refs.chiledContent.kyUnit.length === 0 ? 0 : parseInt(this.$refs.chiledContent.kyUnit[0]),
            jjr: this.$refs.chiledContent.jjr.length === 0 ? 0 : parseInt(this.$refs.chiledContent.jjr[0]),
            jjr_other: this.$refs.chiledContent.jjrOther,
            bg_unit: this.$refs.chiledContent.bjUnit.length === 0 ? 0 : parseInt(this.$refs.chiledContent.bjUnit[0]),
            ky_date: this.$refs.chiledContent.kyDate,
            bj_time: this.$refs.chiledContent.bjTime,
            kyks_time: this.$refs.chiledContent.kyksTime,
            kyjs_time: this.$refs.chiledContent.kyjsTime,
            xz: this.$refs.chiledContent.xz,
            fs_loc: this.$refs.chiledContent.fsqy,
            xc_loc: this.$refs.chiledContent.xcwz,
            xc_locpt: this.$refs.chiledContent.xcwzzb,
            weather_info: this.$refs.chiledContent.tq.length === 0 ? '' : this.$refs.chiledContent.tq[0],
            trend_info: this.$refs.chiledContent.fx.length === 0 ? '' : this.$refs.chiledContent.fx[0],
            temper_info: this.$refs.chiledContent.wd.length === 0 ? '' : this.$refs.chiledContent.wd[0],
            humidity_info: this.$refs.chiledContent.sd.length === 0 ? '' : this.$refs.chiledContent.sd[0],
            light_info: lightInfo,
            bh_flag: this.$refs.chiledContent.bhFlag ? 1 : 0,
            bhr: this.$refs.chiledContent.bhr.length === 0 ? 0 : parseInt(this.$refs.chiledContent.bhr[0]),
            bhr_name: this.$refs.chiledContent.bhrName,
            bhr_unit: this.$refs.chiledContent.bhrdw.length === 0 ? 0 : parseInt(this.$refs.chiledContent.bhrdw[0]),
            bhr_unit_name: this.$refs.chiledContent.bhrUnitName,
            bhr_pos: this.$refs.chiledContent.bhrzw,
            bh_function: this.$refs.chiledContent.bhfs,
            xc_info: this.$refs.chiledContent.xczk.length === 0 ? '' : this.$refs.chiledContent.xczk[0],
            bd_reason: this.$refs.chiledContent.bdyy
          }
          this.$store.commit(SET_RECORDBASE, entity)
          break
        case 2:
          let entity2 = {
            record_reason: this.$refs.chiledContent.recReason,
            xc_disp: this.$refs.chiledContent.disp
          }
          this.$store.commit(SET_RECORDDISP, entity2)
          let loc = {
            east: this.$refs.chiledContent.east,
            west: this.$refs.chiledContent.west,
            north: this.$refs.chiledContent.north,
            south: this.$refs.chiledContent.south
          }
          this.$store.commit(SET_LOC, loc)
          break
        case 3:
          let fileList = []
          this.$refs.chiledContent.completeNames.forEach(function (item) {
            if (item.file_url !== '') {
              let tmp = item
              fileList.push(tmp)
            }
          })
          this.$store.commit(SET_RECORDFILES, fileList)
          break
        case 4:
          let entity4 = {
            jzr: this.$refs.chiledContent.jzr,
            jzr_sex: this.$refs.chiledContent.jzr_sex,
            jzr_birth: this.$refs.chiledContent.jzr_birth,
            jzr_address: this.$refs.chiledContent.jzr_address,
            zhr: this.$refs.chiledContent.zhr.length === 0 ? 0 : parseInt(this.$refs.chiledContent.zhr[0]),
            zhr_name: this.$refs.chiledContent.zhr_name,
            zhr_unit: this.$refs.chiledContent.zhrdw.length === 0 ? 0 : parseInt(this.$refs.chiledContent.zhrdw[0]),
            zhr_unit_name: this.$refs.chiledContent.zhr_unit_name,
            zhr_pos: this.$refs.chiledContent.zhrzw,
            blr: this.$refs.chiledContent.blr.length === 0 ? 0 : parseInt(this.$refs.chiledContent.blr[0]),
            blr_name: this.$refs.chiledContent.blr_name,
            ztr: this.$refs.chiledContent.ztr.length === 0 ? 0 : parseInt(this.$refs.chiledContent.ztr[0]),
            ztr_name: this.$refs.chiledContent.ztr_name,
            zxr: this.$refs.chiledContent.zxr.length === 0 ? 0 : parseInt(this.$refs.chiledContent.zxr[0]),
            zxr_name: this.$refs.chiledContent.zxr_name,
            lxr: this.$refs.chiledContent.lxr.length === 0 ? 0 : parseInt(this.$refs.chiledContent.lxr[0]),
            lxr_name: this.$refs.chiledContent.lxr_name,
            lyr: this.$refs.chiledContent.lyr.length === 0 ? 0 : parseInt(this.$refs.chiledContent.lyr[0]),
            lyr_name: this.$refs.chiledContent.lyr_name
          }
          this.$store.commit(SET_RECORDPERSON, entity4)
          break
      }
    },
    goStep: function (n) {
      if (n === this.currentStep) {
        return
      }
      this.saveCurrentStep(this.currentStep)
      this.currentStep = n
      this.$router.push({name: 'Step' + n, query: {isAdd: this.isAdd}})
    }
  },
  data () {
    return {
      isAdd: '1',
      show: false,
      show2: false,
      showSuccess: false,
      recordNo: '',
      currentStep: 1,
      btnStep: 0,
      step4Selected: false
    }
  }
}
</script>

<style lang="less">
  .btn_wrap {
    padding: 0 1rem;
    margin-top: 1rem;
  }
  @import '~vux/src/styles/reset.less';
  @import '~vux/src/styles/1px.less';
  @import '~vux/src/styles/tap.less';

  body {
    background-color: #fbf9fe;
  }
  html, body {
    height: 100%;
    width: 100%;
    overflow-x: hidden;
  }

  .demo-icon-22 {
    font-family: 'vux-demo';
    font-size: 22px;
    color: #888;
  }
  .weui-tabbar.vux-demo-tabbar {
    /** backdrop-filter: blur(10px);
    background-color: none;
    background: rgba(247, 247, 250, 0.5);**/
  }
  .vux-demo-tabbar .weui-bar__item_on .demo-icon-22 {
    color: #F70968;
  }
  .vux-demo-tabbar .weui-tabbar_item.weui-bar__item_on .vux-demo-tabbar-icon-home {
    color: rgb(53, 73, 94);
  }
  .demo-icon-22:before {
    content: attr(icon);
  }
  .vux-demo-tabbar-component {
    background-color: #F70968;
    color: #fff;
    border-radius: 7px;
    padding: 0 4px;
    line-height: 14px;
  }
  .weui-tabbar__icon + .weui-tabbar__label {
    margin-top: 0!important;
  }
  .vux-demo-header-box {
    z-index: 100;
    position: absolute;
    width: 100%;
    left: 0;
    top: 0;
  }

  @font-face {
    font-family: 'vux-demo';  /* project id 70323 */
    src: url('https://at.alicdn.com/t/font_h1fz4ogaj5cm1jor.eot');
    src: url('https://at.alicdn.com/t/font_h1fz4ogaj5cm1jor.eot?#iefix') format('embedded-opentype'),
    url('https://at.alicdn.com/t/font_h1fz4ogaj5cm1jor.woff') format('woff'),
    url('https://at.alicdn.com/t/font_h1fz4ogaj5cm1jor.ttf') format('truetype'),
    url('https://at.alicdn.com/t/font_h1fz4ogaj5cm1jor.svg#iconfont') format('svg');
  }

  .demo-icon {
    font-family: 'vux-demo';
    font-size:20px;
    color: #04BE02;
  }

  .demo-icon:before {
    content: attr(icon);
  }

  /**
  * vue-router transition
  */
  .router-view {
    width: 100%;
    animation-duration: 0.5s;
    animation-fill-mode: both;
    backface-visibility: hidden;
  }
  .vux-pop-out-enter-active,
  .vux-pop-out-leave-active,
  .vux-pop-in-enter-active,
  .vux-pop-in-leave-active {
    will-change: transform;
    height: 100%;
    position: absolute;
    left: 0;
  }
  .vux-pop-out-enter-active {
    animation-name: popInLeft;
  }
  .vux-pop-out-leave-active {
    animation-name: popOutRight;
  }
  .vux-pop-in-enter-active {
    perspective: 1000;
    animation-name: popInRight;
  }
  .vux-pop-in-leave-active {
    animation-name: popOutLeft;
  }
  @keyframes popInLeft {
    from {
      opacity: 0;
      transform: translate3d(-100%, 0, 0);
    }
    to {
      opacity: 1;
      transform: translate3d(0, 0, 0);
    }
  }
  @keyframes popOutLeft {
    from {
      opacity: 1;
    }
    to {
      opacity: 0;
      transform: translate3d(-100%, 0, 0);
    }
  }
  @keyframes popInRight {
    from {
      opacity: 0;
      transform: translate3d(100%, 0, 0);
    }
    to {
      opacity: 1;
      transform: translate3d(0, 0, 0);
    }
  }
  @keyframes popOutRight {
    from {
      opacity: 1;
    }
    to {
      opacity: 0;
      transform: translate3d(100%, 0, 0);
    }
  }
</style>
