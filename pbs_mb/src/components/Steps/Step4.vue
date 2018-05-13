<template>
  <div>
    <group>
      <x-input title="见证人" v-model='jzr' text-align='right'></x-input>
      <cell title="见证人签名"  is-link @click.native="fetchSingature" ></cell>
      <img v-show="showQm" src=""  id="qmImg" width="300" height="420" >
      <group title="见证人性别">
        <radio :options="sexList" v-model='jzr_sex'></radio>
      </group>
      <datetime v-model="jzr_birth" title="见证人出生日期" min-year=1900 confirm-text="完成" cancel-text="取消" format="YYYY-MM-DD" ></datetime>
      <x-input title="见证人地址" v-model='jzr_address' text-align='right'></x-input>
      <x-input title="指挥人" v-model='zhr_name' text-align='right'>
        <x-button slot="right" type="primary" mini @click.native="showZhrPopup = true">></x-button>
      </x-input>
      <popup-picker title="指挥人单位" :data='unitList' v-model="zhrdw" show-name></popup-picker>
      <x-input title="指挥人单位" v-model='zhr_unit_name' text-align='right'>
        <x-button slot="right" type="primary" mini @click.native="showZhrUnitPopup = true">></x-button>
      </x-input>
      <x-input title="指挥人职位" v-model='zhrzw' text-align='right'></x-input>
      <x-input title="笔录人" v-model='blr_name' text-align='right'>
        <x-button slot="right" type="primary" mini @click.native="showBlrPopup = true">></x-button>
      </x-input>
      <x-input title="制图人" v-model='ztr_name' text-align='right'>
        <x-button slot="right" type="primary" mini @click.native="showZtrPopup = true">></x-button>
      </x-input>
      <x-input title="照相人" v-model='zxr_name' text-align='right'>
        <x-button slot="right" type="primary" mini @click.native="showZxrPopup = true">></x-button>
      </x-input>
      <x-input title="录像人" v-model='lxr_name' text-align='right'>
        <x-button slot="right" type="primary" mini @click.native="showLxrPopup = true">></x-button>
      </x-input>
      <x-input title="录音人" v-model='lyr_name' text-align='right'>
        <x-button slot="right" type="primary" mini @click.native="showLyrPopup = true">></x-button>
      </x-input>
    </group>
    <popup v-model="showZhrPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='userNameList' v-model='zhrPickerName' @on-change='onChangeZhrName'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showZhrPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
    <popup v-model="showZhrUnitPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='unitNameList' v-model='zhrUnitPickerName' @on-change='onChangeZhrUnitName'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showZhrUnitPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
    <popup v-model="showBlrPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='userNameList' v-model='blrPickerName' @on-change='onChangeBlrName'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showBlrPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
    <popup v-model="showZtrPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='userNameList' v-model='ztrPickerName' @on-change='onChangeZtrName'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showZtrPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
    <popup v-model="showZxrPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='userNameList' v-model='zxrPickerName' @on-change='onChangeZxrName'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showZxrPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
    <popup v-model="showLxrPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='userNameList' v-model='lxrPickerName' @on-change='onChangeLxrName'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showLxrPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
    <popup v-model="showLyrPopup" class="checker-popup">
      <div style="padding:10px 10px 10px 10px;">
        <picker :data='userNameList' v-model='lyrPickerName' @on-change='onChangeLyrName'></picker>
      </div>
      <div style="padding: 15px;">
        <x-button @click.native="showLyrPopup = false" plain type="primary"> 确  定 </x-button>
      </div>
    </popup>
  </div>
</template>

<script>
import { Group, XInput, PopupPicker, Picker, XButton, Popup, Cell, Datetime, Radio } from 'vux'
import { SET_RECORDPERSON } from '../../mutationTypes'
export default {
  components: {
    Group,
    PopupPicker,
    XInput,
    Picker,
    XButton,
    Popup,
    Cell,
    Datetime,
    Radio
  },
  mounted: function () {
    this.isAdd = this.$route.query.isAdd
    if(this.$store.getters.GetterEntity.qmBase64Data !== '') {
      var image = document.getElementById('qmImg')
      image.src = this.$store.getters.GetterEntity.qmBase64Data
      this.showQm = true
    }
  },
  computed: {
    unitList: function () {
      return this.$store.getters.GetterUnitState
    },
    userList: function () {
      return this.$store.getters.GetterUserState
    },
    unitNameList: function () {
      return this.$store.getters.GetterUnitNameState
    },
    userNameList: function () {
      return this.$store.getters.GetterUserNameState
    }
  },
  methods: {
    onChangeZhrName (value) {
      this.zhr_name = value
    },
    onChangeZhrUnitName (value) {
      this.zhr_unit_name = value
    },
    onChangeBlrName (value) {
      this.blr_name = value[0]
    },
    onChangeZtrName (value) {
      this.ztr_name = value[0]
    },
    onChangeZxrName (value) {
      this.zxr_name = value[0]
    },
    onChangeLxrName (value) {
      this.lxr_name = value[0]
    },
    onChangeLyrName (value) {
      this.lyr_name = value[0]
    },
    fetchSingature () {
      let entity4 = {
        jzr: this.jzr,
        jzr_sex: this.jzr_sex,
        jzr_birth: this.jzr_birth,
        jzr_address: this.jzr_address,
        zhr: this.zhr.length === 0 ? 0 : parseInt(this.zhr[0]),
        zhr_name: this.zhr_name,
        zhr_unit: this.zhrdw.length === 0 ? 0 : parseInt(this.zhrdw[0]),
        zhr_unit_name: this.zhr_unit_name,
        zhr_pos: this.zhrzw,
        blr: this.blr.length === 0 ? 0 : parseInt(this.blr[0]),
        blr_name: this.blr_name,
        ztr: this.ztr.length === 0 ? 0 : parseInt(this.ztr[0]),
        ztr_name: this.ztr_name,
        zxr: this.zxr.length === 0 ? 0 : parseInt(this.zxr[0]),
        zxr_name: this.zxr_name,
        lxr: this.lxr.length === 0 ? 0 : parseInt(this.lxr[0]),
        lxr_name: this.lxr_name,
        lyr: this.lyr.length === 0 ? 0 : parseInt(this.lyr[0]),
        lyr_name: this.lyr_name
      }
      this.$store.commit(SET_RECORDPERSON, entity4)
      this.$router.push({name: 'OnlineDraw',query: {isAdd: this.isAdd}})
    }
  },
  data () {
    return {
      showZhrPopup: false,
      showZhrUnitPopup: false,
      showBlrPopup: false,
      showZtrPopup: false,
      showZxrPopup: false,
      showLxrPopup: false,
      showLyrPopup: false,
      showQm: false,
      zhrPickerName: [''],
      zhrUnitPickerName: [''],
      blrPickerName: [''],
      ztrPickerName: [''],
      zxrPickerName: [''],
      lxrPickerName: [''],
      lyrPickerName: [''],
      sexList: ['男','女'],
      zhr: this.$store.getters.GetterEntity.zhr === 0 ? [] : [this.$store.getters.GetterEntity.zhr + ''],
      zhr_name: this.$store.getters.GetterEntity.zhr_name,
      zhrdw: this.$store.getters.GetterEntity.zhr_unit === 0 ? [] : [this.$store.getters.GetterEntity.zhr_unit + ''],
      zhr_unit_name: this.$store.getters.GetterEntity.zhr_unit_name,
      blr: this.$store.getters.GetterEntity.blr === 0 ? [] : [this.$store.getters.GetterEntity.blr + ''],
      blr_name: this.$store.getters.GetterEntity.blr_name,
      ztr: this.$store.getters.GetterEntity.ztr === 0 ? [] : [this.$store.getters.GetterEntity.ztr + ''],
      ztr_name: this.$store.getters.GetterEntity.ztr_name,
      zxr: this.$store.getters.GetterEntity.zxr === 0 ? [] : [this.$store.getters.GetterEntity.zxr + ''],
      zxr_name: this.$store.getters.GetterEntity.zxr_name,
      lxr: this.$store.getters.GetterEntity.lxr === 0 ? [] : [this.$store.getters.GetterEntity.lxr + ''],
      lxr_name: this.$store.getters.GetterEntity.lxr_name,
      lyr: this.$store.getters.GetterEntity.lyr === 0 ? [] : [this.$store.getters.GetterEntity.lyr + ''],
      lyr_name: this.$store.getters.GetterEntity.lyr_name,
      jzr: this.$store.getters.GetterEntity.jzr,
      jzr_sex: this.$store.getters.GetterEntity.jzr_sex,
      jzr_birth: this.$store.getters.GetterEntity.jzr_birth,
      jzr_address: this.$store.getters.GetterEntity.jzr_address,
      zhrzw: this.$store.getters.GetterEntity.zhr_pos,
      isAdd: '1'
    }
  }
}
</script>

<style>
.vux-demo {
  text-align: center;
}
.logo {
  width: 100px;
  height: 100px
}
</style>
