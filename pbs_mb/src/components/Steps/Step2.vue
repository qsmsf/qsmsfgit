<template >
  <div ref="mainGroup">
    <group>
      <x-button type="primary" @click.native="show = true" mini>自动生成勘查事由模板</x-button>
      <h4>勘查事由:</h4>
      <x-textarea  placeholder="请填写勘查事由"  :rows="3" v-model="recReason" autosize></x-textarea>
      <h4>现场描述:</h4>
      <x-input title="现场东面:" v-model="east" text-align='right'></x-input>
      <x-input title="现场南面:" v-model="south" text-align='right'></x-input>
      <x-input title="现场西面:" v-model="west" text-align='right'></x-input>
      <x-input title="现场北面:" v-model="north" text-align='right'></x-input>
      <x-button type="primary" @click.native="show2 = true" mini>自动生成现场描述模板</x-button>
      <x-button type="primary" @click.native="showHint = true" mini>插入描述模板</x-button>
      <x-textarea  placeholder="请填写现场描述"  :rows="4" v-model="disp" @on-focus="onGetFocus(0)" autosize ></x-textarea>
    </group>
    <confirm v-model="show" title="自动生成" @on-confirm="generData1" >
      <p style="text-align:center;">点击确定后勘查事由将会替换已有内容!</p>
    </confirm>
    <confirm v-model="show2" title="自动生成" @on-confirm="generData2" >
      <p style="text-align:center;">点击确定后现场描述将会替换已有内容!</p>
    </confirm>
    <div v-transfer-dom>
      <x-dialog v-model="showHint" class="dialog-demo">
        <p class="dialog-title">描述模板</p>
        <group>
          <cell @click.native="onGetModel('');" is-link>
            <span slot="title" style="color:green;">模板一</span>
          </cell>
          <cell @click.native="onGetModel('');" is-link>
            <span slot="title" style="color:green;">模板二</span>
          </cell>
        </group>
        <div @click="showHint=false">
          <span class="vux-close"></span>
        </div>
      </x-dialog>
    </div>
  </div>
</template>

<script>
import { Group, XTextarea, Cell, PopupPicker, XButton, Confirm, XInput,
  XDialog, TransferDomDirective as TransferDom } from 'vux'

export default {
  directives: {
    TransferDom
  },
  components: {
    Group,
    PopupPicker,
    Cell,
    XButton,
    Confirm,
    XInput,
    XTextarea,
    XDialog
  },
  mounted: function () {
    if (this.recReason === '') {
      this.generData1()
    }
  },
  methods: {
    generData1: function () {
      let entity = this.$store.getters.GetterEntity
      let bjTime = new Date(entity.bj_time)
      this.recReason = this.getDateStrCH(bjTime) + ',' + this.$store.getters.GetterMe.member.unit_name + '接到事主报警：' + entity.fs_loc
      this.recReason += entity.xc_loc + '发生一宗' + entity.xz + '案。' + '需技术员对现场进行勘查。'
    },
    generData2: function () {
      let entity = this.$store.getters.GetterEntity
      this.disp = '  现场位于' + entity.fs_loc + entity.xc_loc + ',现场'
      if (this.east !== '') {
        this.disp += '东侧为' + this.east + '，'
      }
      if (this.south !== '') {
        this.disp += '南侧为' + this.south + '，'
      }
      if (this.west !== '') {
        this.disp += '西侧为' + this.west + '，'
      }
      if (this.north !== '') {
        this.disp += '北侧为' + this.north
      }
      this.disp += '。\n    中心现场位于'
    },
    getDateStrCH: function (t) {
      return t.getFullYear() + '年' + (t.getMonth() + 1) + '月' + t.getDate() + '日' + t.getHours() + '时' + t.getMinutes() + '分'
    },
    onGetFocus (p) {
      /* this.$refs.mainDiv.animate({
        scrollTop: 500},
        100, function () {
        // stuff to do after animation is complete
        }) */
      this.$refs.mainGroup.scrollTop = 0
    },
    onGetModel (txt) {
      this.disp += txt
      this.showHint = false
    }
  },
  data () {
    return {
      show: false,
      show2: false,
      showHint: false,
      east: this.$store.getters.GetterLoc.east,
      south: this.$store.getters.GetterLoc.south,
      north: this.$store.getters.GetterLoc.north,
      west: this.$store.getters.GetterLoc.west,
      recReason: this.$store.getters.GetterEntity.record_reason,
      disp: this.$store.getters.GetterEntity.xc_disp
    }
  }
}
</script>

<style lang="less" scoped>
@import '~vux/src/styles/close';

.dialog-demo {
  .weui-dialog{
    border-radius: 8px;
    padding-bottom: 8px;
  }
  .dialog-title {
    line-height: 30px;
    color: #666;
  }
  .vux-close {
    margin-top: 8px;
    margin-bottom: 8px;
  }
  .dialog-box {
    height: 500px;
    width: 400px;
    padding:15px 0;
    overflow:'scroll';
    -webkit-overflow-scrolling: 'touch';
  }
}
</style>
