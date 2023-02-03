import React from 'react'
import { useState, useEffect } from 'react'
import Team from '../components/Team'
import { Button } from 'react-bootstrap'
import { Form } from 'react-bootstrap'
import SearchIcon from '../img/icons8-search-30.png'

function AllTeamsPage(props) {

    const [show, setShow] = useState(false)
    const [teams, setTeams] = useState([])//[...teams, newTeam]
    const [inputText, setInputText] = useState("");
    const [showTeams, setShowTeams] = useState([])

//load teams/sve timove koji traze clanove ili svoje timove

    useEffect(() => {
        const request = {
            method: 'GET',
            headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}   
        }
        if(teams.length == 0)
        {
            fetch('https://localhost:7013/Team/GetOtherActiveTeams', request).then( (response) => {
                if(response.ok){
                    response.json().then((teams) => {
                        setTeams(teams)
                        setShowTeams(teams)

                    })
                }
            })
        }
        setShowTeams(teams.filter((team) => {
            if(inputText === "")
            {
                console.log(inputText + "hehe")

                return team;
            }
            else{
                console.log(inputText)
                return team.ime.toLowerCase().includes(inputText);
            }
        })
        )

    }, [inputText])

    const inputHandler = (e) => {
        var lowerCase = e.toLowerCase();
  
        setInputText(lowerCase);
    };

    //potrebna ispravka
    return (
        <div className='teamList'>

            <div className="search">
                <Form.Group className='form-cont'>
                    <Form.Label><h2>Search</h2></Form.Label>
                    <div className='searchBar'>
                    <img className="searchIcon" src={SearchIcon}/>
                    <Form.Control type='text' placeholder='Type here...'
                        value = {inputText} onChange= { (e) => 
                            inputHandler(e.target.value) }/>
                    </div>
                </Form.Group>

            </div>

            <div className="container">
            {showTeams.map((team) => (<Team key={team.id} tm={team} isOwn={false}/>))}
            </div>
        </div>
    )

}

export default AllTeamsPage

/*<>
teams.map((team) => {
    <Team id={team.id}, bla = {team.bola}
})
</>*/