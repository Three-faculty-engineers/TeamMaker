import React from 'react'
import { useState, useEffect } from 'react'
import Team from '../components/Team'
import { Form } from 'react-bootstrap'
import SearchIcon from '../img/icons8-search-30.png'


//ova strana mozda moze da se sjedini sa 
function OwnTeamsPage() {

    const [show, setShow] = useState(false)
    const [teams, setTeams] = useState([])//[...teams, newTeam]
    const [showTeams, setShowTeams] = useState([])
    const [inputText, setInputText] = useState("");

//load teams/sve timove koji traze clanove ili svoje timove
    const request = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}   
    }
    
    const inputHandler = (e) => {
      var lowerCase = e.toLowerCase();

      setInputText(lowerCase);
    };


    useEffect(() => {

        if(teams.length == 0)
        {
            fetch(`https://localhost:7013/Team/GetOwnTeams`, request).then((response) => {
                if(response.ok)
                {
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
                return team;
            }
            else{
                return team.ime.toLowerCase().includes(inputText);
            }
        }))

    }, [inputText])



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

            <div className='teamList2ilitakonest'>
                {showTeams.map((team) => (<Team key={team.id} tm={team} isOwn={true}/>))}
            </div>

        </div>

    )

}

export default OwnTeamsPage

/*<>
teams.map((team) => {
    <Team id={team.id}, bla = {team.bola}
})
</>*/