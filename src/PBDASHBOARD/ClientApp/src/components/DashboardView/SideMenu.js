import React, {Component, useState} from 'react';
import {
    ProSidebar,
    Menu,
    MenuItem,
    SidebarHeader,
    SidebarFooter,
    SidebarContent,
} from "react-pro-sidebar";
import {Link} from 'react-router-dom';

//import icons from react icons
import {FaList, FaRegHeart} from "react-icons/fa";
import {FiHome, FiLogOut, FiArrowLeftCircle, FiArrowRightCircle} from "react-icons/fi";
import {RiPencilLine} from "react-icons/ri";

//import sidebar css from react-pro-sidebar module and our custom css 
import "react-pro-sidebar/dist/css/styles.css";
import "./Home.css";
import {BiCaretRightCircle} from "react-icons/bi";


export class SideMenu extends Component {
    static displayName = SideMenu.name;


    render() {

        return (
            <><div id="header">
                    <ProSidebar>
                        <SidebarHeader>
                            <div className="logotext">
                                <p>{"Publication Dashboard"}</p>
                            </div>

                        </SidebarHeader>
                        <SidebarContent>
                            <Menu iconShape="square">
                                <MenuItem active={false} icon={<FiHome/>}>
                                    <Link to='/'>
                                    </Link>
                                    Home
                                </MenuItem>

                                <MenuItem icon={<FaList/>}>
                                    <Link to='/Publication'>

                                    </Link>Publication</MenuItem>

                                <MenuItem icon={<FaList/>}>
                                    <Link to='/Institute'>

                                    </Link>Institutes</MenuItem>
                                <MenuItem icon={<BiCaretRightCircle/>}>
                                    <Link to='/Facult'>

                                    </Link>
                                    Faculty</MenuItem>
                                {<MenuItem icon={<FaList/>}>
                                    <Link to='/Samarbied'>

                                    </Link>Collaboration</MenuItem>}

                                <MenuItem icon={<BiCaretRightCircle/>}>
                                    <Link to='/GroupVsPublications'>

                                    </Link>
                                    Research Groups</MenuItem>
                            </Menu>
                        </SidebarContent>


                    </ProSidebar>


                </div>

            </>

        );
    }

}
  