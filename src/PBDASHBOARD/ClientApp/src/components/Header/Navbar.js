import React, { useState } from 'react';
import {Link, Redirect, withRouter} from 'react-router-dom';
import './Navbar.css';
let buttons;
const NavMenu = (props)=> {
    console.log(props);
    /*this is going to change or update the state, and once clicked, it will change its state to true*/
    const [click, setClick] = useState(false);
    const [dropdown, setDropdown] = useState(false); /*creating the dropdown*/

    /*set the state to be opposite of what it is*/
    const handleClick = () => setClick(!click);
    const closeMobileMenu = () => setClick(false);


    /**/
    const onMouseEnter = () => {
        if (window.innerWidth < 960) {
            setDropdown(false);
        } else {
            setDropdown(true);
        }
    };


    const onMouseLeave = () => {
        if (window.innerWidth < 960) {
            setDropdown(false);
        } else {
            setDropdown(false);
        }
    };
    const handleLogins=()=> {

        localStorage.clear();

        // window.location.reload()




    };
    const extendElement = () => {
        dropdown ? setDropdown(false) : setDropdown(true);
    };
    let user= localStorage.getItem('user');
    if(user){
        return (<><nav className='navbar'>
                <Link to='/' className='navbar-logo' onClick={closeMobileMenu}>Dashboard<i className='fab fa-firstdraft' />
                </Link ><div className='menu-icon' onClick={handleClick}>
                <i className={click ? 'fas fa-times' : 'fas fa-bars'} />
            </div>
                <ul className={click ? 'nav-menu active' : 'nav-menu'}>

                    <li className='nav-item'>
                        <Link
                            to='/RegistrationForm'
                            className='nav-links'
                            onClick={closeMobileMenu}
                        >
                            Registration
                        </Link>
                    </li><li className='nav-item'>
                    <Link
                        to='/Editing'
                        className='nav-links'
                        onClick={closeMobileMenu}
                    >Edit
                    </Link>
                </li><li className='nav-item'>
                    <Link
                        to='/Logout'
                        className='nav-links'
                        onClick={closeMobileMenu}>Logout</Link></li></ul>
            </nav>
            </>
        );

    }
    return (
        <><nav className='navbar'><Link to='/' className='navbar-logo' onClick={closeMobileMenu}>
            Dashboard
            <i className='fab fa-firstdraft' />
        </Link>
            <div className='menu-icon' onClick={handleClick}>
                <i className={click ? 'fas fa-times' : 'fas fa-bars'} />
            </div>
            <ul className={click ? 'nav-menu active' : 'nav-menu'}>
                <li className='nav-item'>
                    <Link
                        to='/Login'
                        className='nav-links'
                        onClick={closeMobileMenu}
                    >
                        login
                    </Link>
                </li><li className='nav-item'>
                <Link
                    to='/RegistrationForm'
                    className='nav-links'
                    onClick={closeMobileMenu}>
                    Registration
                </Link>
            </li><li className='nav-item'>
                <Link
                    to='/Editing'
                    className='nav-links'
                    onClick={closeMobileMenu}>
                    Edit
                </Link>
            </li>



            </ul>
        </nav>
        </>
    );
}

export default NavMenu;