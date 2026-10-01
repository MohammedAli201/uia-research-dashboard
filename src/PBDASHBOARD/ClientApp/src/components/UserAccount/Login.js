import React, {Component} from "react";
import axios from "axios";

export default class Login extends Component {
    constructor(props) {
        super(props);
        this.state = {
            email: "",
            password: "",
            loginErrors: ""};
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleChange = this.handleChange.bind(this);}
    handleChange=(event) =>{
        this.setState({
            [event.target.name]: event.target.value
        });};

    handleSubmit=(event)=> {
        event.preventDefault();
        const {email, password} = this.state;
        axios
            .post(window.location.origin + "/AuthMangement/Login",{
                    email: email,
                    password: password,},
                {withCredentials: true})
            .then(response => {
                console.log("The login process is successed ");
                if (response.data.success === true) {
                    console.log(response.data);
                    let token= response.data.token;
                    localStorage.setItem('jwt',token);
                    localStorage.setItem('user', JSON.stringify(response.data));
                    this.props.history.push("/");
                    this.props.handleLogin(true)
                }
            })
            .catch(error => {
                console.log("login error", error);
            });};

    render() {
        let user= localStorage.getItem('user');
        if (user){
            this.props.history.push("/")
        }
        return (
            <div style={{padding: '15rem', width: 800, marginBottom: '2rem', marginLeft: '12rem', height: '250px',}}>
                <form onSubmit={this.handleSubmit}>
                    <h3>login </h3>
                    <div className="form-group">
                        <label> Your Email</label>
                        <input type="email" className="form-control" name="email" placeholder="Email"
                               value={this.state.email} onChange={this.handleChange} required/></div>
                    <div className="form-group">
                        <label>Password</label>
                        <input type="password" className="form-control" name="password" placeholder="Password"
                               value={this.state.password} onChange={this.handleChange} required/>
                    </div>
                    <button type="submit" className="btn btn-dark btn-lg btn-block">Submit</button>
                </form>
            </div>
        );
    }
}