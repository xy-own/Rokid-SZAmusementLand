#ifndef __COLOUR__
    #define __COLOUR__

    fixed3 HSVToRGB( fixed3 c )
    {
        float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
        fixed3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
        return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
    }
    
    fixed3 RGBToHSV(fixed3 c)
    {
        float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
        float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
        float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
        float d = q.x - min( q.w, q.y );
        float e = 1.0e-10;
        return fixed3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
    }
#endif