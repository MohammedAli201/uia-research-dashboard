import React, {Component} from 'react';
import Navbar from "./components/Header/Navbar";
import './App.css';
import {BrowserRouter as Router, Switch, Route} from 'react-router-dom';
import {Institute} from './components/Institution/Institutes';
import {Faculty} from './components/Institution/Faculty';
import {PresentationVideo} from './components/DashboardView/PresentationVideo';
import Login from "./components/UserAccount/Login";
import {GroupVsPublications} from "./components/Researches/GroupVsPublications";
import {SideMenu} from "./components/DashboardView/SideMenu";
import {Samarbied} from "./components/Researches/Samarbied";
import {RegistrationForm} from "./components/UserAccount/RegistrationForm";
import Logout from "./components/UserAccount/Logout";
import Editing from "./components/UserAccount/Edit";
import {Home} from "./components/DashboardView/Home";
import {Publication} from "./components/Researches/Publication";


export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);

        this.state = {
            loggedInStatus: false
        };

        this.handleLogin = this.handleLogin.bind(this)
        this.logoutHandler= this.logoutHandler.bind(this);
        this.logout= this.logout.bind(this);

    }

    handleLogin(islogin) {
        this.setState({loggedInStatus:islogin});


    }


    componentDidMount() {
    }
    logoutHandler =() => {
        this.setState({loggedInStatus:true});

    };

    logout = () => {

        this.setState({loggedInStatus:true});
        localStorage.clear();
    };


    render() {
        let user = localStorage.getItem('user');


        if (!user) {

            return (
                <Router>
                    <Switch>
                        <React.Fragment>

                            <div>
                                <Navbar  />
                                <SideMenu/>


                                <div>
                                    {/*exact takes u to exactly the place it was told to. in this case, Home*/}
                                    <Route path='/sideMenu' component={SideMenu}/>
                                    <Route path='/' exact component={Home }/>
                                    <Route path='/Institute' component={Institute}/>
                                    <Route path='/Facult' component={Faculty}/>
                                    <Route path='/presentation' component={PresentationVideo}/>
                                    {/*
                                    <Route path='/pubVsTime' component={PubVsTime}/>
*/}
                                    <Route path='/Samarbied' component={Samarbied}/>
                                    <Route path='/RegistrationForm' component={RegistrationForm}/>
                                    <Route path='/Login'  render={(props)=><Login {...props}  handleLogin={this.handleLogin}/>}/>
                                    <Route path='/Editing' component={Editing}/>}
                                    <Route path='/Publication' component={Publication}/>
                                    <Route path='/GroupVsPublications' component={GroupVsPublications}/>


                                </div>

                            </div>

                        </React.Fragment>
                    </Switch>
                </Router>
            );

        }

        return (
            <Router>
                <Switch>
                    <React.Fragment>

                        <div>
                            <Navbar/>
                            <SideMenu/>
                            <div>
                                <Route path='/sideMenu' component={SideMenu}/>
                                <Route path='/' exact component={Home }/>
                                <Route path='/Institute' component={Institute}/>
                                <Route path='/Facult' component={Faculty}/>
                                <Route path='/presentation' component={PresentationVideo}/>
                                {/*
                                <Route path='/pubVsTime' component={PubVsTime}/>
*/}
                                <Route path='/Samarbied' component={Samarbied}/>
                                <Route path='/GroupVsPublications' component={GroupVsPublications}/>
                                <Route path='/Publication' component={Publication}/>

                                <Route path='/RegistrationForm' component={RegistrationForm}/>*
                                <Route path='/Editing' component={Editing}/>}
                                <Route path='/Logout'  render={(props)=><Logout {...props}  handleLogin={this.handleLogin}/>}/>

                            </div>


                        </div>

                    </React.Fragment>
                </Switch>
            </Router>
        );
    }
}