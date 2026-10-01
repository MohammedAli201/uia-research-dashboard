import React from "react";
import { ReactVideo } from "reactjs-media";

export const PresentationVideo = () => {
    return (
        <div  style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            marginCenter: 'center',
            marginInline: 'center'

        }}>
            <ReactVideo
                src="https://www.example.com/url_to_video.mp4"
                poster="https://www.example.com/poster.png"
                primaryColor="red"
                // other props
                
            />
        </div>
    );
};