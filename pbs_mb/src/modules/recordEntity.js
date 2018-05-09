// content
import {
  SET_RECORDBASE,
  SET_RECORDDISP,
  SET_RECORDPERSON,
  SET_RECORDOTHER,
  SET_RECORDFILES,
  SET_MAPZB,
  CLR_RECORDINFO,
  SET_LOC
} from '../mutationTypes'

const state = {
  // 保存第一页数据
  entity: {
    record_id: 0,
    uuid: '',
    record_no: '',
    record_title: '',
    ky_unit: 0,
    bg_unit: 0,
    ky_date: '',
    jjr: 0,
    jjr_other: '',
    bj_time: '',
    kyks_time: '',
    kyjs_time: '',
    dest_unit: 0,
    xz: '',
    fs_loc: '',
    xc_loc: '',
    xc_locpt: '',
    record_reason: '',
    xc_disp: '',
    weather_info: '',
    temper_info: '',
    trend_info: '',
    humidity_info: '',
    light_info: '',
    bh_flag: 0,
    bhr: 0,
    bhr_unit: 0,
    bhr_pos: '',
    bh_function: '',
    xc_info: '',
    bd_reason: '',
    jzr: '',
    jzr_sex: '',
    jzr_birth: '',
    jzr_address: '',
    zhr: 0,
    zhr_name: '',
    zhr_unit: 0,
    zhr_unit_name: '',
    zhr_pos: '',
    blr: 0,
    blr_name: '',
    ztr: 0,
    ztr_name: '',
    zxr: 0,
    zxr_name: '',
    lxr: 0,
    lxr_name: '',
    lyr: 0,
    lyr_name: '',
    creater_id: 0,
    bhr_name: '',
    bhr_unit_name: '',
    east: '',
    west: '',
    north: '',
    south: ''
  },
  fileList: []
}

const mutations = {
  // 设置 token 登录名 头像
  [SET_RECORDBASE] (state, data) {
    try {
      state.entity.record_title = data.xc_loc + '-' + data.xz
      state.entity.ky_unit = data.ky_unit
      state.entity.bg_unit = data.bg_unit
      state.entity.ky_date = data.ky_date
      state.entity.jjr = data.jjr
      state.entity.jjr_other = data.jjr_other
      state.entity.bj_time = data.bj_time
      state.entity.kyks_time = data.kyks_time
      state.entity.kyjs_time = data.kyjs_time
      state.entity.xz = data.xz
      state.entity.fs_loc = data.fs_loc
      state.entity.xc_loc = data.xc_loc
      state.entity.weather_info = data.weather_info
      state.entity.temper_info = data.temper_info
      state.entity.trend_info = data.trend_info
      state.entity.humidity_info = data.humidity_info
      state.entity.light_info = data.light_info
      state.entity.bh_flag = data.bh_flag
      state.entity.bhr = data.bhr
      state.entity.bhr_unit = data.bhr_unit
      state.entity.bhr_pos = data.bhr_pos
      state.entity.bh_function = data.bh_function
      state.entity.xc_info = data.xc_info
      state.entity.bd_reason = data.bd_reason
      state.entity.bhr_name = data.bhr_name
      state.entity.bhr_unit_name = data.bhr_unit_name
    } catch (err) {
      console.log(err)
    }
  },
  [SET_RECORDDISP] (state, data) {
    try {
      state.entity.record_reason = data.record_reason
      state.entity.xc_disp = data.xc_disp
    } catch (err) {
      console.log(err)
    }
  },
  [SET_RECORDPERSON] (state, data) {
    try {
      state.entity.jzr = data.jzr
      state.entity.jzr_sex = data.jzr_sex
      state.entity.jzr_birth = data.jzr_birth
      state.entity.jzr_address = data.jzr_address
      state.entity.zhr = data.zhr
      state.entity.zhr_name = data.zhr_name
      state.entity.zhr_unit = data.zhr_unit
      state.entity.zhr_unit_name = data.zhr_unit_name
      state.entity.zhr_pos = data.zhr_pos
      state.entity.blr = data.blr
      state.entity.ztr = data.ztr
      state.entity.zxr = data.zxr
      state.entity.lxr = data.lxr
      state.entity.lyr = data.lyr
      state.entity.blr_name = data.blr_name
      state.entity.ztr_name = data.ztr_name
      state.entity.zxr_name = data.zxr_name
      state.entity.lxr_name = data.lxr_name
      state.entity.lyr_name = data.lyr_name
    } catch (err) {
      console.log(err)
    }
  },
  [SET_RECORDOTHER] (state, data) {
    try {
      state.entity.record_id = data.record_id
      state.entity.creater_id = data.creater_id
      state.entity.uuid = data.uuid
      state.entity.record_no = data.record_no
    } catch (err) {
      console.log(err)
    }
  },
  [SET_RECORDFILES] (state, data) {
    try {
      state.fileList = data
    } catch (err) {
      console.log(err)
    }
  },
  [SET_MAPZB] (state, data) {
    try {
      state.entity.xc_locpt = data
    } catch (err) {
      console.log(err)
    }
  },
  [SET_LOC] (state, data) {
    try {
      state.entity.east = data.east
      state.entity.west = data.west
      state.entity.south = data.south
      state.entity.north = data.north
    } catch (err) {
      console.log(err)
    }
  },
  [CLR_RECORDINFO] (state, data) {
    state.entity = {
      record_id: 0,
      uuid: '',
      record_title: '',
      record_no: '',
      ky_unit: data.ky_unit,
      bg_unit: data.bg_unit,
      ky_date: data.ky_date,
      jjr: data.jjr,
      jjr_other: '',
      bj_time: data.bj_time,
      bjr: '',
      bjr_sex: 0,
      bjr_idcard: '',
      bjr_phoneNo: '',
      kyks_time: data.kyks_time,
      kyjs_time: '',
      dest_unit: 0,
      xz: '',
      fs_loc: data.fs_loc,
      xc_loc: '',
      xc_locpt: '',
      record_reason: '',
      xc_disp: '',
      weather_info: data.weather_info,
      temper_info: data.temper_info,
      trend_info: data.trend_info,
      humidity_info: data.humidity_info,
      light_info: data.light_info,
      bh_flag: false,
      bhr: data.bhr,
      bhr_unit: data.bhr_unit,
      bhr_pos: data.bhr_pos,
      bh_function: data.bh_function,
      xc_info: data.xc_info,
      bd_reason: data.bd_reason,
      jzr: '',
      jzr_sex: '',
      jzr_birth: '',
      jzr_address: '',
      zhr: data.zhr,
      zhr_name: data.zhr_name,
      zhr_unit: data.zhr_unit,
      zhr_unit_name: data.zhr_unit_name,
      zhr_pos: data.zhr_pos,
      blr: data.blr,
      ztr: data.ztr,
      zxr: data.zxr,
      lxr: data.lxr,
      lyr: 0,
      blr_name: data.blr_name,
      ztr_name: data.ztr_name,
      zxr_name: data.zxr_name,
      lxr_name: data.lxr_name,
      lyr_name: '',
      creater_id: 0,
      bhr_name: data.bhr_name,
      bhr_unit_name: data.bhr_unit_name,
      east: '',
      west: '',
      north: '',
      south: ''
    }
    state.fileList = []
  }
}
const actions = {

}

const getters = {
  GetterEntity: state => {
    return state.entity
  },
  GetterFileList: state => {
    return state.fileList
  },
  GetterRecord: state => {
    return state
  },
  GetterLoc: state => {
    var loc = {
      east: state.entity.east,
      west: state.entity.west,
      south: state.entity.south,
      north: state.entity.north
    }
    return loc
  },
  GetterMapLoc: state => {
    return state.entity.xc_locpt
  }
}

export default {
  state,
  mutations,
  actions,
  getters
}
