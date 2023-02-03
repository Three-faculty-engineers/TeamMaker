import React from 'react'
import { useState, useEffect } from 'react'

function About(props) {

    return (
        <div className='singleTeam'>
        <h2>ABOUT</h2>
        <p>
        Welcome to TEAMMAKER, an independent project made by four university students for a school assignment!
        We hope you enjoy your stay and find this website easy to navigate.
        </p>
        <p>
        The minds behind this website are four Computer Science majors: Dimitrije Filipović, Anđela Ilić, Veljko Turudić, and Tamara Ilić.
        All four students are on their third year of studies.
        </p>
        <p>
        This website was created for university students to find like-minded partners with whom they could work on
        various projects, programming-related or not. Any user can create a team and describe what kind of
        members said team is looking for, or apply to join somebody else's team if they so desire.
        </p>
        <p>For any further questions or feedback of any sort, you can contact tamarailic@elfak.rs.</p>
        </div>
    )

}

export default About