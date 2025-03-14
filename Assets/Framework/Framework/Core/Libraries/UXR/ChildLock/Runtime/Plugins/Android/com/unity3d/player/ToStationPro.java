package com.unity3d.player;

import android.content.Context;
import android.provider.Settings;
import android.view.KeyEvent;

public class ToStationPro {
    private Context unityContext = null;

    public void setContext(Context context) {
        unityContext = context;
    }

    public void appLock(String packName) {
        StringBuilder lockedStr = new StringBuilder();
        lockedStr.append(packName);
        lockedStr.append(":").append(KeyEvent.KEYCODE_POWER);
        lockedStr.append(":").append(KeyEvent.KEYCODE_HOME);
        lockedStr.append(":").append(KeyEvent.KEYCODE_MENU);
//        lockedStr.append(":").append(KeyEvent.KEYCODE_VOLUME_DOWN);
//        lockedStr.append(":").append(KeyEvent.KEYCODE_VOLUME_UP);
//        lockedStr.append(":").append(KeyEvent.KEYCODE_BRIGHTNESS_DOWN);
//        lockedStr.append(":").append(KeyEvent.KEYCODE_BRIGHTNESS_UP);
        Settings.Global.putString(unityContext.getContentResolver(), "sys_key_locked_app", lockedStr.toString());
    }

    public void appUnLock() {
        Settings.Global.putString(unityContext.getContentResolver(), "sys_key_locked_app", "");
    }
}