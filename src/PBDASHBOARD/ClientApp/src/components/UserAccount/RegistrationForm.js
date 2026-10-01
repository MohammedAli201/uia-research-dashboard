import React, {Component} from "react";
import Autocomplete from "@material-ui/lab/Autocomplete";
import Checkbox from "@material-ui/core/Checkbox";
import TextField from "@material-ui/core/TextField";
import {Link, Redirect} from "react-router-dom";
import CheckBoxOutlineBlankIcon from "@material-ui/icons/CheckBoxOutlineBlank";
import CheckBoxIcon from "@material-ui/icons/CheckBox";
import axios from "axios";
import Select from "react-select";


const icon = <CheckBoxOutlineBlankIcon fontSize="small"/>;
const checkedIcon = <CheckBoxIcon fontSize="small"/>;
let role="";
let grName=[];

const Role=[{
    value:1,
    label:"Admin"},{
    value:2,
    label:"Member"},
];
export class RegistrationForm extends Component {
    constructor(props) {
        super(props);
        this.state = {
            email: '',
            password:'',
            name:'',
            groupName:'',
            nameOfGroups:[]
            
            
            

        };
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleChecked = this.handleChecked.bind(this);
        this.validation = this.validation.bind(this)
        this.handleRole = this.handleRole.bind(this)
        
    }

  async  handleSubmit(event) {
      event.preventDefault();
        const {

            email,
            password,
            name,
            groupName,
            
        }= this.state;
        
         let token = localStorage.getItem("jwt");
     
      const data ={
          name,email,groupName,password
      };

      const headers = {
          'Content-Type': 'application/json',
          'Authorization': 'bearer '+ token
      };
      
      

       await axios.post('/AuthMangement/Register', data, {
            headers:headers
        }).then(resp=>{
           if (resp.data.success === true) {
               this.props.history.push("/");

               this.props.handleLogin();

               event.preventDefault();
           }

           console.log(resp)
        }).catch(error=>{
            console.log(error, "Error has occured")
        });
      
      console.log(headers)
   
        
        
        
    }

    handleChange(event) {
        this.setState({
            [event.target.name]: event.target.value
        })
    }
    handleChecked(event){
        this.setState({
            groupName:event.target.value
        })
        console.log(this.state.groupName)
    }
  async  handleNameOFGroups(){
        if (this.state.nameOfGroups.length===0){
            const response = await fetch('AuthMangement/NameOfGroups');/*i can put the url to the API here*/
            const nameGroups = await response.json();
            for (let item in nameGroups){
                if (nameGroups[item]!==null){
                   grName.push(nameGroups[item])
                }
            }
            this.setState({nameOfGroups: grName});
        }
      
         console.log(this.state.nameOfGroups)
    }
  
   async componentDidMount() {
       await this.handleNameOFGroups()
    }
    validation(){
      
       
    }
    handleRole(event){
        if(event!==undefined){
            console.log(event.label);
            console.log("Hello");
        }
            
   
    }

    render() {
        let user= localStorage.getItem('user');
        if (!user){
            this.props.history.push("/Login")
        }
        
        return (
            <div style={{padding: '15rem', width: 800, marginBottom: '2rem', marginLeft: '12rem', height: '250px',}}>
                <form onSubmit={this.handleSubmit}>

                        <h3>Registration </h3>

                    <div className="form-group">
                        <label> Contributor's Email</label>
                        <input type="email" className="form-control" name="email" placeholder="Email"
                               value={this.state.email} onChange={this.handleChange} required/>
                    </div><div className="form-group">
                        <label> Password</label>
                        <input type="password" className="form-control" name="password" placeholder="Password"
                               value={this.state.password} onChange={this.handleChange} required/>
                    </div>
                    <div className="form-group">
                        <label>Name of Contributor</label>
                         <input type="text" className="form-control" name="name" placeholder="Name of Contributor" 
                               value={this.state.name} onChange={this.handleChange} required/> </div>

                    <div className="form-group">
                        <label>Role</label>
                        <Select options={Role} onChange={this.handleRole } require/>
                        </div>

                    <div className="form-group">
                        <label>Groupe Name</label>
                        <Autocomplete  /*is a normal text input enhanced by a panel of suggested options.*/
                            multiple  /*this helps us have multiple results in the search bar or we can search for multiple things at once*/
                            id="checkboxes-tags-demo"
                            options={this.state.nameOfGroups}
                            renderOption={(option, {selected}) => (
                                <React.Fragment>
                                    <Checkbox
                                        icon={icon}
                                        checkedIcon={checkedIcon}
                                        style={{marginRight: 8}}
                                        checked={selected}
                                        value={option}
                                        required

                                        onChange={this.handleChecked}
                                    />{option}
                                </React.Fragment>)}

                            renderInput={(params) => (
                                <TextField {...params} variant="outlined"
                                           label="Checkboxes"
                                           placeholder="Favorites"/>)}/>
                        <div><Link to='/faculty-charts'/></div>
                   
                    </div>
               


                    <button type="submit" className="btn btn-dark btn-lg btn-block">Submit</button>
                </form>
            </div>
    
        )
    }
}