import React from 'react'
import { useState, useEffect } from 'react'
import { Button } from 'react-bootstrap'
import DeleteIcon from '../img/icons8-delete-30.png'
import { Form } from 'react-bootstrap'
import SearchIcon from '../img/icons8-search-30.png'


function AdminPage(props) {

    const [teams, setTeams] = useState([])
    const [users, setUsers] = useState([])

    const [showTeams, setShowTeams] = useState([])
    const [inputTextTeams, setInputTextTeams] = useState("");

    const [showUsers, setShowUsers] = useState([])
    const [inputTextUsers, setInputTextUsers] = useState("");

    const inputHandlerTeams = (e) => {
        var lowerCase = e.toLowerCase();
        setInputTextTeams(lowerCase);
    };

    const inputHandlerUsers = (e) => {
        var lowerCase = e.toLowerCase();
        setInputTextUsers(lowerCase);
    };
    

    const request = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
    } 
    const deleteRequest = {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
    } 

    useEffect(() => {


        if(teams.length == 0)
            fetch('https://localhost:7013/Admin/GetAllTeams', request).then( (response) => {
                if(response.ok){
                    response.json().then((teams) => {
                        setShowTeams(teams)
                        setTeams(teams)
                    })
                }
            })

        if(users.length == 0)
            fetch('https://localhost:7013/Admin/GetAllKorisnici', request).then( (response) => {
                if(response.ok){
                    response.json().then((users) => {
                        setShowUsers(users)
                        setUsers(users)
                    })
                }
            })


        setShowTeams(teams.filter((team) => {
            if(inputTextTeams === "")
            {
                return team;
            }
            else{
                return team.ime.toLowerCase().includes(inputTextTeams);
            }
        }))

        setShowUsers(users.filter((Users) => {
            if(inputTextUsers === "")
            {
                return Users;
            }
            else{
                return Users.username.toLowerCase().includes(inputTextUsers);
            }
        }))
    }, [inputTextUsers, inputTextTeams])
    




    const deleteKorisnik = (id) => {
        fetch('https://localhost:7013/Admin/DeleteKorisnik/' + id, deleteRequest).then( (response) => {
            if(response.ok){
                alert("korisnik je obrisan")
                setUsers(users.filter((user) => user.id != id))
            }
        })
    }

    const DeleteTeam = (id) => {
        fetch('https://localhost:7013/Admin/DeleteTeam/' + id, deleteRequest).then( (response) => {
            if(response.ok){
                alert("korisnik je obrisan")
                setTeams(teams.filter((team) => team.id != id))
            }
        })
    }



    return (
        <div className='admin'>


            <h2>TEAMS</h2>
            
            <div className="search">
                <Form.Group className='form-cont'>
                    <Form.Label><h2>Search Teams</h2></Form.Label>
                    <div className='searchBar'>
                    <img className="searchIcon" src={SearchIcon}/>
                    <Form.Control type='text' placeholder='Type here...'
                        value = {inputTextTeams} onChange= { (e) => 
                            inputHandlerTeams(e.target.value) }/>
                    </div>
                </Form.Group>
            </div>
            
            <div className='sprintlist'>
                {showTeams.map((team) => (<div className='tinyOption' key={team.id}><p>{team.ime}</p> <Button variant='dark' onClick={() => {DeleteTeam(team.id)}}> <img className="PanelIcon" src={DeleteIcon}/> DELETE</Button> </div>))}
            </div>


            <h2>USERS:</h2>

            <div className="search">
            <Form.Group className='form-cont'>
                    <Form.Label><h2>Search Users</h2></Form.Label>
                    <div className='searchBar'>
                    <img className="searchIcon" src={SearchIcon}/>
                    <Form.Control type='text' placeholder='Type here...'
                        value = {inputTextUsers} onChange= { (e) => 
                            inputHandlerUsers(e.target.value) }/>
                    </div>
            </Form.Group>
            </div>

            <div className='sprintlist'>
                {showUsers.map((user) => (<div className='tinyOption' key={user.id}><p>{user.username}</p> <Button variant='dark' onClick={() => {deleteKorisnik(user.id)}}> <img className="PanelIcon" src={DeleteIcon}/> DELETE</Button> </div>))}
            </div>


        </div>
    )

}

export default AdminPage